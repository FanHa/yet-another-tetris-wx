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

        /// <summary>动作成功进入，Enter() 已执行；订阅者不改变动作生命周期或重入 Runner 调度。</summary>
        public event Action<UnitActionType> OnActionStarted;

        /// <summary>动作执行正常结束，已撤下并执行 Exit() 清理；订阅者不重入 Runner 调度。</summary>
        public event Action<UnitActionType> OnActionCompleted;

        /// <summary>动作调度被取消，已撤下并执行 Exit() 清理；不撤销业务效果，原调用返回后仍可能发出 Committed。订阅者不重入 Runner 调度。</summary>
        public event Action<UnitActionType> OnActionCanceled;

        /// <summary>攻击已发射或技能执行正常返回；不保证产生效果、命中或效果结束，也可能在 Canceled 后发出。订阅者不重入 Runner 调度。</summary>
        public event Action<UnitActionType> OnActionCommitted;

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

        private void HandleActionCommitted(UnitActionType actionType)
        {
            OnActionCommitted?.Invoke(actionType);
        }

        public void Tick(global::Units.Actions.ActionTickContext context)
        {
            if (!HasCurrentAction)
            {
                TryStartNextEligibleAction();
            }
            else
            {
                currentAction.Tick(context);
            }

            if (HasCurrentAction && currentAction.IsExecutionFinished)
            {
                CompleteCurrentAction();
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

            OnActionStarted?.Invoke(startedAction.Type);

            return true;
        }

        public void CancelCurrentAction()
        {
            if (!HasCurrentAction)
            {
                return;
            }

            var action = currentAction;
            currentAction = null;
            action.Exit();
            OnActionCanceled?.Invoke(action.Type);
        }

        private void CompleteCurrentAction()
        {
            var action = currentAction;
            currentAction = null;
            action.Exit();
            OnActionCompleted?.Invoke(action.Type);
        }
    }
}
