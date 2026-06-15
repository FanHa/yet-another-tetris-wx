using UnityEngine;

namespace Units.Actions
{
    public sealed class MoveAction : UnitAction
    {
        public override int Priority => 0;

        private const float HitAndRunMinRatio = 0.9f;
        private const float AllyProtectDistancePadding = 0.1f;

        public MoveAction(Unit owner) : base(owner, UnitActionType.Move)
        {
        }

        public override bool CanStart()
        {
            return TryGetMoveTarget(out _);
        }

        protected override void OnEnter()
        {
            if (!TryGetMoveTarget(out var target))
            {
                Complete();
                return;
            }

            GetMoveDistanceRange(target, out float minDistance, out float maxDistance);
            Owner.ApplyMovement(Movement.MovementRequest.PathfindToTarget(target.transform, minDistance, maxDistance));

            // Move action is a one-frame command; movement continues in NavMeshAgent.
            Complete();
        }

        private void GetMoveDistanceRange(Unit target, out float minDistance, out float maxDistance)
        {
            switch (Owner.CurrentMoveBehaviorMode)
            {
                case Unit.MoveBehaviorMode.TowardAlly:
                    // 靠近队友时尽量贴近，方便保护与协同。
                    minDistance = 0f;
                    maxDistance = Owner.BodyRadius + target.BodyRadius + AllyProtectDistancePadding;
                    break;
                case Unit.MoveBehaviorMode.TowardEnemy:
                default:
                    maxDistance = Owner.Attributes.AttackRange.finalValue;
                    minDistance = maxDistance * HitAndRunMinRatio;
                    break;
            }
        }

        private bool TryGetMoveTarget(out Unit target)
        {
            switch (Owner.CurrentMoveBehaviorMode)
            {
                case Unit.MoveBehaviorMode.TowardAlly:
                    if (Owner.TryGetClosestAlly(out target))
                    {
                        return true;
                    }

                    return Owner.TryGetClosestEnemy(out target);
                case Unit.MoveBehaviorMode.TowardEnemy:
                default:
                    return Owner.TryGetClosestEnemy(out target);
            }
        }
    }
}
