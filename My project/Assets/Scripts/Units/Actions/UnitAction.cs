using System;

namespace Units.Actions
{
    public abstract class UnitAction
    {
        protected readonly IUnitActionContext Context;
        public event Action<UnitActionType, UnitActionCommitKind> OnCommitted;

        public UnitActionType Type { get; }
        public bool IsCompleted { get; protected set; }
        public virtual int Priority => 0;

        protected UnitAction(IUnitActionContext context, UnitActionType type)
        {
            Context = context;
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

        public void Tick(global::Units.Actions.ActionTickContext context)
        {
            if (IsCompleted)
            {
                return;
            }

            OnTick(context);
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

        protected void RaiseCommitted(UnitActionCommitKind commitKind)
        {
            OnCommitted?.Invoke(Type, commitKind);
        }

        protected abstract void OnEnter();

        protected virtual void OnTick(global::Units.Actions.ActionTickContext context)
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
