using System;

namespace Units.Actions
{
    public abstract class UnitAction
    {
        public event Action<UnitActionType> OnCommitted;

        private bool isExecuting;

        public UnitActionType Type { get; }
        public bool IsExecutionFinished { get; private set; }
        public virtual int StartSelectionPriority => 0;

        protected UnitAction(UnitActionType type)
        {
            Type = type;
        }

        public virtual bool CanStart()
        {
            return true;
        }

        public void Enter()
        {
            IsExecutionFinished = false;
            isExecuting = true;
            OnEnter();
        }

        public void Tick(global::Units.Actions.ActionTickContext context)
        {
            if (!isExecuting || IsExecutionFinished)
            {
                return;
            }

            OnTick(context);
        }

        public void Exit()
        {
            isExecuting = false;
            OnExit();
        }

        protected void FinishExecution()
        {
            // 业务回调可能已同步取消动作，返回后不再标记为正常完成。
            if (isExecuting)
            {
                IsExecutionFinished = true;
            }
        }

        protected void RaiseCommitted()
        {
            OnCommitted?.Invoke(Type);
        }

        protected abstract void OnEnter();

        protected virtual void OnTick(global::Units.Actions.ActionTickContext context)
        {
        }

        protected virtual void OnExit()
        {
        }
    }

    public abstract class UnitAction<TContext> : UnitAction
    {
        protected readonly TContext Context;

        protected UnitAction(TContext context, UnitActionType type) : base(type)
        {
            Context = context;
        }
    }
}
