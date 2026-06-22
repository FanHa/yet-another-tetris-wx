using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction
    {
        public override int Priority => 10;

        private Unit pendingTarget;
        private bool hasLaunchedProjectile;
        private bool hasMarkedAttackExecuted;

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
            Owner.ApplyAnimationCommand(new AnimationController.PlayAttackAnimationCommand(direction));
            hasLaunchedProjectile = false;
            hasMarkedAttackExecuted = false;
        }

        protected override void OnTick()
        {
            var visualStatus = Owner.GetActionVisualStatus(AnimationController.ActionVisualType.Attack, out float progress);

            if (visualStatus == AnimationController.ActionVisualStatus.Playing)
            {
                if (!hasLaunchedProjectile && progress >= Owner.GetAttackHitPhase())
                {
                    if (pendingTarget != null && pendingTarget.IsActive)
                    {
                        Owner.ExecuteAttackProjectile(pendingTarget, Owner.Attributes.AttackPower.finalValue);
                    }

                    hasLaunchedProjectile = true;
                    if (!hasMarkedAttackExecuted)
                    {
                        Owner.MarkAttackExecuted();
                        hasMarkedAttackExecuted = true;
                    }
                }

                return;
            }

            if (visualStatus == AnimationController.ActionVisualStatus.NotStarted)
            {
                return;
            }

            if (!hasMarkedAttackExecuted)
            {
                Owner.MarkAttackExecuted();
                hasMarkedAttackExecuted = true;
            }

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
            Owner.ApplyAnimationCommand(new AnimationController.StopActionAnimationCommand());
        }
    }
}
