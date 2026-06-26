using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Actions
{
    public sealed class UnitActionRunner
    {
        private readonly Unit owner;
        private readonly List<UnitAction> actions;
        private UnitAction currentAction;

        private bool HasCurrentAction => currentAction != null;

        /// <summary>动作成功进入，Enter() 已执行。</summary>
        public event Action<UnitActionType> OnActionStarted;

        /// <summary>动作按预期完成，Exit() 已执行。</summary>
        public event Action<UnitActionType> OnActionCompleted;

        /// <summary>动作被外部状态（眩晕/强制位移/Deactivate）中止。</summary>
        public event Action<UnitActionType> OnActionCanceled;

        /// <summary>动作被更高优先级动作抢占。</summary>
        public event Action<UnitActionType> OnActionInterrupted;

        public UnitActionRunner(Unit owner)
        {
            this.owner = owner;
            actions = CreateDefaultActions(owner)
                .OrderByDescending(action => action.Priority)
                .ToList();
        }

        public void Tick(global::Units.Actions.ActionTickContext context)
        {
            SynchronizeOwnerState();
            bool startedActionThisTick = TryStartHighestPriority();

            if (!HasCurrentAction)
            {
                return;
            }

            if (startedActionThisTick)
            {
                return;
            }

            var action = currentAction;
            action.Tick(context);

            // Tick 内可能触发中断/回收，导致 currentAction 被清空或替换。
            if (currentAction != action)
            {
                return;
            }

            if (action.IsCompleted)
            {
                ExitCurrent();
            }
        }

        private void SynchronizeOwnerState()
        {
            if (!HasCurrentAction)
            {
                return;
            }

            if (ShouldYieldToOwnerState(currentAction))
            {
                CancelCurrentInternal(interrupted: false);
            }
        }

        private bool TryStartHighestPriority()
        {
            foreach (var action in actions)
            {
                if (TryStart(action))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryStart(UnitAction action)
        {
            if (!action.CanStart())
            {
                return false;
            }

            if (HasCurrentAction)
            {
                if (!action.CanPreempt(currentAction))
                {
                    return false;
                }

                CancelCurrentInternal(interrupted: true);
            }

            currentAction = action;
            var startedAction = currentAction;
            startedAction.Enter();

            if (currentAction != startedAction)
            {
                return true;
            }

            if (startedAction.IsCompleted)
            {
                ExitCurrent();
                return true;
            }

            OnActionStarted?.Invoke(startedAction.Type);

            return true;
        }

        public void OnOwnerDeactivated()
        {
            if (!HasCurrentAction)
            {
                return;
            }

            CancelCurrentInternal(interrupted: false);
        }

        private void ExitCurrent()
        {
            var action = currentAction;
            currentAction = null;
            action.Exit();
            OnActionCompleted?.Invoke(action.Type);
        }

        private void CancelCurrentInternal(bool interrupted)
        {
            var action = currentAction;
            currentAction = null;
            action.Cancel();
            if (interrupted)
                OnActionInterrupted?.Invoke(action.Type);
            else
                OnActionCanceled?.Invoke(action.Type);
        }

        private bool ShouldYieldToOwnerState(UnitAction action)
        {
            if (owner.IsStunned && action.Type != UnitActionType.Stun)
            {
                return true;
            }

            if (owner.IsSkillMotionActive && action.Type != UnitActionType.SkillMotion)
            {
                return true;
            }

            return false;
        }

        

        private static IEnumerable<UnitAction> CreateDefaultActions(Unit owner)
        {
            yield return new StunAction(owner);
            yield return new SkillMotionAction(owner);
            yield return new CastSkillAction(owner);
            yield return new AttackAction(owner);
            yield return new MoveAction(owner);
        }
    }
}
