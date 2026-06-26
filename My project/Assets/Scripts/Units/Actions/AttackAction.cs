using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction
    {
        public override int Priority => 10;

        private Unit pendingTarget;
        private bool hasLaunchedProjectile;
        private bool hasMarkedAttackExecuted;
        private readonly ActionTimelineProgress attackTimeline = new();

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

        // Timeline mapping for AttackAction:
        // speed        <- Unit.GetActionTimelineSpeed()
        // duration     <- Unit.GetAttackActionDurationSeconds()
        private float GetAttackTimelineDurationSeconds() => Owner.GetAttackActionDurationSeconds();

        private float AdvanceAttackTimeline(global::Units.Actions.ActionTickContext context)
        {
            return attackTimeline.Advance(
            context.DeltaTime,
            context.TimelineSpeed,
                GetAttackTimelineDurationSeconds());
        }

        private bool HasCompletedAttackTimeline(float timelineProgress)
        {
            return timelineProgress >= 1f;
        }

        private void TryLaunchProjectile()
        {
            if (pendingTarget != null && pendingTarget.IsActive)
            {
                Owner.ExecuteAttackProjectile(pendingTarget, Owner.Attributes.AttackPower.finalValue);
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
            attackTimeline.Reset();
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            float timelineProgress = AdvanceAttackTimeline(context);

            if (!HasCompletedAttackTimeline(timelineProgress))
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
