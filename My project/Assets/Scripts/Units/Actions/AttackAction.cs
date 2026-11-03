using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction<IAttackActionContext>
    {
        public override int StartSelectionPriority => 10;

        private float attackElapsedSeconds;
        private Unit attackTarget;

        public AttackAction(IAttackActionContext context) : base(context, UnitActionType.Attack)
        {
        }

        public override bool CanStart()
        {
            attackTarget = null;

            if (Context.ActionSpeed <= 0f)
            {
                return false;
            }

            if (!Context.CanAttackNow())
            {
                return false;
            }

            if (!Context.TryGetClosestEnemy(out var target) || !IsAttackableTarget(target))
            {
                return false;
            }

            attackTarget = target;
            return true;
        }

        private void AdvanceAttackTimeline(global::Units.Actions.ActionTickContext context)
        {
            attackElapsedSeconds += context.DeltaTime * Mathf.Max(0f, context.TimelineSpeed);
        }

        private bool HasCompletedAttackTimeline()
        {
            float durationSeconds = Mathf.Max(0.001f, Context.GetAttackActionDurationSeconds());
            return attackElapsedSeconds >= durationSeconds;
        }

        private bool IsAttackableTarget(Unit target)
        {
            if (target == null || !target.IsActive)
            {
                return false;
            }

            float distance = Vector2.Distance(Context.Position, target.transform.position);
            return distance <= Context.GetEffectiveAttackRangeTo(target);
        }

        protected override void OnEnter()
        {
            Context.SuspendAutoMovement();
            attackElapsedSeconds = 0f;
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            AdvanceAttackTimeline(context);

            if (!HasCompletedAttackTimeline())
            {
                return;
            }

            if (IsAttackableTarget(attackTarget))
            {
                Context.ExecuteAttack(attackTarget);
                RaiseCommitted();
            }

            FinishExecution();
        }

        protected override void OnExit()
        {
            Context.ResumeAutoMovement();
            attackTarget = null;
        }
    }
}
