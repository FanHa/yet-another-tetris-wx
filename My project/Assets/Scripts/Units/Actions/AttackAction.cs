using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction, IAnimationEventHandler
    {
        public override int Priority => 10;

        private Unit pendingTarget;
        private int animationToken;

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

        protected override void OnEnter()
        {
            if (!Owner.TryGetClosestEnemy(out pendingTarget) || pendingTarget == null)
            {
                Complete();
                return;
            }

            Owner.PauseNavigation();
            Vector2 direction = (pendingTarget.transform.position - Owner.transform.position).normalized;
            var result = Owner.ApplyAnimationCommand(new AnimationController.PlayAttackAnimationCommand(direction));
            animationToken = result.token;
        }

        protected override void OnExit()
        {
            pendingTarget = null;
            Owner.ResumeNavigation();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            Owner.ApplyAnimationCommand(new AnimationController.StopActionAnimationCommand());
        }

        public void HandleAnimationEvent(AnimationEventType eventType)
        {
            if (eventType != AnimationEventType.AttackEnd)
            {
                Debug.LogWarning($"[AttackAction] Unexpected animation event ignored. unit={Owner.name}, event={eventType}, expected={AnimationEventType.AttackEnd}, token={animationToken}, time={Time.time:F3}");
                return;
            }

            if (IsCompleted)
            {
                return;
            }

            if (!Owner.IsAnimationTokenCurrent(animationToken))
            {
                Debug.LogWarning($"[AttackAction] Stale animation event ignored. unit={Owner.name}, event={eventType}, expectedToken={animationToken}, time={Time.time:F3}");
                return;
            }

            if (pendingTarget != null && pendingTarget.IsActive)
            {
                Owner.ExecuteAttackProjectile(pendingTarget, Owner.Attributes.AttackPower.finalValue);
            }

            Owner.MarkAttackExecuted();
            Complete();
        }
    }
}
