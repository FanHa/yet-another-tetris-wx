using System.Collections.Generic;
using System.Linq;

namespace Units.Actions
{
    public sealed class UnitActionRunner
    {
        private readonly List<UnitAction> actions;

        public UnitAction CurrentAction { get; private set; }

        public bool IsBusy => CurrentAction != null;

        public UnitActionRunner(Unit owner)
        {
            actions = CreateDefaultActions(owner)
                .OrderByDescending(action => action.Priority)
                .ToList();
        }

        public void Tick()
        {
            TryStartHighestPriority();

            if (!IsBusy)
            {
                return;
            }

            CurrentAction.Tick();
            if (CurrentAction.IsCompleted)
            {
                ExitCurrent();
            }
        }

        private void TryStartHighestPriority()
        {
            foreach (var action in actions)
            {
                if (TryStart(action))
                {
                    return;
                }
            }
        }

        private bool TryStart(UnitAction action)
        {
            if (!action.CanStart())
            {
                return false;
            }

            if (IsBusy)
            {
                if (!action.CanPreempt(CurrentAction))
                {
                    return false;
                }

                CancelCurrentInternal();
            }

            CurrentAction = action;
            CurrentAction.Enter();

            if (CurrentAction.IsCompleted)
            {
                ExitCurrent();
            }

            return true;
        }

        

        public void NotifyAttackAnimationEnd()
        {
            if (CurrentAction is IAttackAnimationEndHandler handler)
            {
                handler.HandleAttackAnimationEnd();
            }
        }

        public void NotifySkillCastAnimationEnd()
        {
            if (CurrentAction is ISkillCastAnimationEndHandler handler)
            {
                handler.HandleSkillCastAnimationEnd();
            }
        }

        public void CancelCurrent()
        {
            if (!IsBusy)
            {
                return;
            }

            CancelCurrentInternal();
        }

        private void ExitCurrent()
        {
            var action = CurrentAction;
            CurrentAction = null;
            action.Exit();
        }

        private void CancelCurrentInternal()
        {
            var action = CurrentAction;
            CurrentAction = null;
            action.Cancel();
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
