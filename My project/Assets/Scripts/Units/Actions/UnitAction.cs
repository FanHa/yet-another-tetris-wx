namespace Units.Actions
{
    public abstract class UnitAction
    {
        protected readonly Unit Owner;

        public UnitActionType Type { get; }
        public bool IsCompleted { get; protected set; }
        public virtual int Priority => 0;

        protected UnitAction(Unit owner, UnitActionType type)
        {
            Owner = owner;
            Type = type;
        }

        public virtual bool CanStart()
        {
            return true;
        }

        public virtual bool CanPreempt(UnitAction currentAction)
        {
            return false;
        }

        public void Enter()
        {
            IsCompleted = false;
            OnEnter();
        }

        public void Tick()
        {
            if (IsCompleted)
            {
                return;
            }

            OnTick();
        }

        public void Exit()
        {
            OnExit();
        }

        public void Cancel()
        {
            IsCompleted = true;
            OnCancel();
        }

        protected void Complete()
        {
            IsCompleted = true;
        }

        protected abstract void OnEnter();

        protected virtual void OnTick()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnCancel()
        {
            OnExit();
        }
    }
}
