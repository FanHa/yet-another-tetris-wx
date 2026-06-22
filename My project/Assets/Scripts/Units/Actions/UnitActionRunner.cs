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

        public UnitActionRunner(Unit owner)
        {
            this.owner = owner;
            actions = CreateDefaultActions(owner)
                .OrderByDescending(action => action.Priority)
                .ToList();
        }

        public void Tick()
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
            action.Tick();

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
                CancelCurrentInternal();
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

                CancelCurrentInternal();
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
            }

            return true;
        }

        public void OnOwnerDeactivated()
        {
            if (!HasCurrentAction)
            {
                return;
            }

            CancelCurrentInternal();
        }

        private void ExitCurrent()
        {
            var action = currentAction;
            currentAction = null;
            action.Exit();
        }

        private void CancelCurrentInternal()
        {
            var action = currentAction;
            currentAction = null;
            action.Cancel();
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
