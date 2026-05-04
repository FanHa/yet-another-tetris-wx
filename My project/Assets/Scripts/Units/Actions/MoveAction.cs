using UnityEngine;

namespace Units.Actions
{
    public sealed class MoveAction : UnitAction
    {
        public override int Priority => 0;

        private const float HitAndRunMinRatio = 0.9f;

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

            if (Owner.Attributes.CanHitAndRun)
            {
                float maxDistance = Owner.Attributes.AttackRange.finalValue;
                float minDistance = maxDistance * HitAndRunMinRatio;
                Owner.Movement.MoveToDistanceFromTarget(enemy.transform, minDistance, maxDistance);
            }
            else
            {
                Owner.Movement.MoveAlongPathToTarget(enemy);
            }

            // Move action is a one-frame command; movement continues in NavMeshAgent.
            Complete();
        }
    }
}
