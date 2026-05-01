using UnityEngine;

namespace Units.Actions
{
    public sealed class AttackAction : UnitAction
    {
        public override int Priority => 10;

        private Unit pendingTarget;

        public AttackAction(Unit owner) : base(owner, UnitActionType.Attack)
        {
        }

        public override bool CanStart()
        {
            if (!Owner.CanAttackNow())
            {
                return false;
            }

            if (!Owner.TryGetClosestEnemy(out var target))
            {
                return false;
            }

            float distance = Vector2.Distance(Owner.transform.position, target.transform.position);
            return distance <= Owner.Attributes.AttackRange.finalValue;
        }

        protected override void OnEnter()
        {
            if (!Owner.TryGetClosestEnemy(out pendingTarget) || pendingTarget == null)
            {
                Complete();
                return;
            }

            Owner.Movement.PauseNavigation();
            Vector2 direction = (pendingTarget.transform.position - Owner.transform.position).normalized;
            Owner.AnimationController.SetLookDirection(direction);
            Owner.AnimationController.TriggerAttack();
        }

        protected override void OnExit()
        {
            pendingTarget = null;
            Owner.Movement.ResumeNavigation();
        }

        public void HandleAnimationEnd()
        {
            if (IsCompleted)
            {
                return;
            }

            if (pendingTarget != null)
            {
                float distance = Vector2.Distance(Owner.transform.position, pendingTarget.transform.position);
                if (distance <= Owner.Attributes.AttackRange.finalValue)
                {
                    Owner.ExecuteAttackProjectile(pendingTarget, Owner.Attributes.AttackPower.finalValue);
                }
                else
                {
                    // TODO: miss handling.
                }
            }

            Owner.MarkAttackExecuted();
            Complete();
        }
    }
}
