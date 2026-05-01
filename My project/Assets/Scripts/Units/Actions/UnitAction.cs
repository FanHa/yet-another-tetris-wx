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
    }
}
