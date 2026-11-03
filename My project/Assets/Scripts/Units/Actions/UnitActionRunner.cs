using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Actions
{
    public sealed class UnitActionRunner
    {
        private readonly List<UnitAction> actions;
        private UnitAction currentAction;

        private bool HasCurrentAction => currentAction != null;

        /// <summary>动作成功进入，Enter() 已执行。</summary>
        public event Action<UnitActionType> OnActionStarted;

        /// <summary>动作按预期完成，Exit() 已执行。</summary>
        public event Action<UnitActionType> OnActionCompleted;

        /// <summary>动作被调用方取消，Cancel() 已执行。</summary>
        public event Action<UnitActionType> OnActionCanceled;

        /// <summary>动作业务效果已提交（例如发射投射物、执行技能）。</summary>
        public event Action<UnitActionType, UnitActionCommitKind> OnActionCommitted;

        public UnitActionRunner(IEnumerable<UnitAction> actions)
        {
            this.actions = actions
                .OrderByDescending(action => action.StartSelectionPriority)
                .ToList();

            foreach (var action in this.actions)
            {
                action.OnCommitted += HandleActionCommitted;
            }
        }

        private void HandleActionCommitted(UnitActionType actionType, UnitActionCommitKind commitKind)
        {
            OnActionCommitted?.Invoke(actionType, commitKind);
        }

        public void Tick(global::Units.Actions.ActionTickContext context)
        {
            if (!HasCurrentAction)
            {
                TryStartNextEligibleAction();
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

        private void TryStartNextEligibleAction()
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

            currentAction = action;
            var startedAction = currentAction;
            startedAction.Enter();

            if (currentAction != startedAction)
            {
                return true;
            }

            OnActionStarted?.Invoke(startedAction.Type);

            if (currentAction != startedAction)
            {
                return true;
            }

            if (startedAction.IsCompleted)
            {
                ExitCurrent();
                return true;
            }

            return true;
        }

        public void CancelCurrentAction()
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
            OnActionCompleted?.Invoke(action.Type);
        }

        private void CancelCurrentInternal()
        {
            var action = currentAction;
            currentAction = null;
            action.Cancel();
            OnActionCanceled?.Invoke(action.Type);
        }
    }
}
