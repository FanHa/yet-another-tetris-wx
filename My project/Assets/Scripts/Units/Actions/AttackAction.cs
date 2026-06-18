using UnityEngine;

namespace Units.Actions
{
    public interface IAttackAnimationEndHandler
    {
        void HandleAttackAnimationEnd();
    }

    public sealed class AttackAction : UnitAction, IAttackAnimationEndHandler
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
            var result = Owner.ApplyAnimation(new AnimationController.PlayAttackAnimationCommand(direction));
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
            Owner.ApplyAnimation(new AnimationController.StopActionAnimationCommand());
        }

        public void HandleAttackAnimationEnd()
        {
            if (IsCompleted || !Owner.IsAnimationTokenCurrent(animationToken))
            {
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
