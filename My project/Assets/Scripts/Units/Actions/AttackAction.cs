using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction
    {
        public override int Priority => 10;

        private Unit pendingTarget;
        private bool hasLaunchedProjectile;
        private bool hasMarkedAttackExecuted;
        private float attackElapsedSeconds;

        public AttackAction(Unit owner) : base(owner, UnitActionType.Attack)
        {
        }

        public override bool CanStart()
        {
            if (Owner.Attributes.ActionSpeed.finalValue <= 0f)
            {
                return false;
            }

            if (!Owner.CanAttackNow())
            {
                return false;
            }

            if (!Owner.TryGetClosestEnemy(out var target))
            {
                return false;
            }

            float distance = Vector2.Distance(Owner.transform.position, target.transform.position);
            return distance <= Owner.GetEffectiveAttackRangeTo(target);
        }

        private void AdvanceAttackTimeline(global::Units.Actions.ActionTickContext context)
        {
            attackElapsedSeconds += context.DeltaTime * Mathf.Max(0f, context.TimelineSpeed);
        }

        private bool HasCompletedAttackTimeline()
        {
            float durationSeconds = Mathf.Max(0.001f, Owner.GetAttackActionDurationSeconds());
            return attackElapsedSeconds >= durationSeconds;
        }

        private void TryLaunchProjectile()
        {
            if (pendingTarget != null && pendingTarget.IsActive)
            {
                Owner.ExecuteAttackProjectile(pendingTarget);
            }
        }

        private void MarkAttackExecutedIfNeeded()
        {
            if (hasMarkedAttackExecuted)
            {
                return;
            }

            Owner.MarkAttackExecuted();
            hasMarkedAttackExecuted = true;
        }

        protected override void OnEnter()
        {
            if (!Owner.TryGetClosestEnemy(out pendingTarget) || pendingTarget == null)
            {
                Complete();
                return;
            }

            Owner.PauseNavigation();
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
            Owner.ResumeNavigation();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
        }
    }
}
