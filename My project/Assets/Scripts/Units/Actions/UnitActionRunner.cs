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

        public bool TryStart(UnitAction action)
        {
            if (action == null || !action.CanStart())
            {
                return false;
            }

            if (IsBusy)
            {
                bool canPreemptCurrent = action.Type == UnitActionType.Stun
                    && CurrentAction.Type != UnitActionType.Stun;

                if (!canPreemptCurrent)
                {
                    return false;
                }

                CompleteCurrent();
            }

            CurrentAction = action;
            CurrentAction.Enter();

            if (CurrentAction.IsCompleted)
            {
                CompleteCurrent();
            }

            return true;
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
                CompleteCurrent();
            }
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

            CompleteCurrent();
        }

        private void CompleteCurrent()
        {
            var action = CurrentAction;
            CurrentAction = null;
            action.Exit();
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

        private static IEnumerable<UnitAction> CreateDefaultActions(Unit owner)
        {
            yield return new SkillMotionAction(owner);
            yield return new CastSkillAction(owner);
            yield return new AttackAction(owner);
            yield return new MoveAction(owner);
        }
    }
}
