using System.Linq;

namespace Units.Actions
{
    public sealed class UnitActionRunner
    {
        public UnitAction CurrentAction { get; private set; }

        public bool IsBusy => CurrentAction != null;

        public bool TryStart(UnitAction action)
        {
            if (action == null || IsBusy || !action.CanStart())
            {
                return false;
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

        public void TryStartHighestPriority(params UnitAction[] candidates)
        {
            if (IsBusy)
            {
                return;
            }

            foreach (var action in candidates.OrderByDescending(a => a.Priority))
            {
                if (TryStart(action))
                {
                    return;
                }
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
    }
}
