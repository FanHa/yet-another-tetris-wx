using UnityEngine;

namespace Units.Actions
{
    public sealed class MoveAction : UnitAction
    {
        public override int Priority => 0;

        public MoveAction(Unit owner) : base(owner, UnitActionType.Move)
        {
        }

        public override bool CanStart()
        {
            return Owner.TryGetClosestEnemy(out _);
        }

        protected override void OnEnter()
        {
            if (!Owner.TryGetClosestEnemy(out var enemy))
            {
                Complete();
                return;
            }

            Transform targetTransform = enemy.transform;
            if (Owner.Attributes.CanHitAndRun)
            {
                float maxDistance = Owner.Attributes.AttackRange.finalValue;
                float minDistance = maxDistance * 0.9f;
                Owner.Movement.MoveToDistanceFromTarget(targetTransform, minDistance, maxDistance);
            }
            else
            {
                Owner.Movement.MoveAlongPathToTarget(targetTransform);
            }

            // Move action is a one-frame command; movement continues in NavMeshAgent.
            Complete();
        }
    }
}
