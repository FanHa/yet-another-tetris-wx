using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction<IAttackActionContext>
    {
        public override int Priority => 10;

        private Unit pendingTarget;
        private bool hasLaunchedProjectile;
        private bool hasMarkedAttackExecuted;
        private float attackElapsedSeconds;

        public AttackAction(IAttackActionContext context) : base(context, UnitActionType.Attack)
        {
        }

        public override bool CanStart()
        {
            if (Context.ActionSpeed <= 0f)
            {
                return false;
            }

            if (!Context.CanAttackNow())
            {
                return false;
            }

            if (!Context.TryGetClosestEnemy(out var target))
            {
                return false;
            }

            float distance = Vector2.Distance(Context.Position, target.transform.position);
            return distance <= Context.GetEffectiveAttackRangeTo(target);
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

        private void TryLaunchProjectile()
        {
            if (pendingTarget != null && pendingTarget.IsActive)
            {
                Context.ExecuteAttackProjectile(pendingTarget);
                RaiseCommitted(UnitActionCommitKind.AttackProjectileLaunched);
            }
        }

        private void MarkAttackExecutedIfNeeded()
        {
            if (hasMarkedAttackExecuted)
            {
                return;
            }

            Context.MarkAttackExecuted();
            hasMarkedAttackExecuted = true;
        }

        protected override void OnEnter()
        {
            if (!Context.TryGetClosestEnemy(out pendingTarget) || pendingTarget == null)
            {
                Complete();
                return;
            }

            Context.SuspendAutoMovement();
            hasLaunchedProjectile = false;
            hasMarkedAttackExecuted = false;
            attackElapsedSeconds = 0f;
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            AdvanceAttackTimeline(context);

            if (!HasCompletedAttackTimeline())
            {
                return;
            }

            if (!hasLaunchedProjectile)
            {
                TryLaunchProjectile();
                hasLaunchedProjectile = true;
            }

            MarkAttackExecutedIfNeeded();

            Complete();
        }

        protected override void OnExit()
        {
            pendingTarget = null;
            Context.ResumeAutoMovement();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
        }
    }
}
