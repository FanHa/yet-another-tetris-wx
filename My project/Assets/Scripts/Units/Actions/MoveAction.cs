using UnityEngine;

namespace Units.Actions
{
    public sealed class MoveAction : UnitAction
    {
        public override int Priority => 0;

        private const float HitAndRunMinRatio = 0.9f;
        private const float AllyProtectDistancePadding = 0.1f;

        public MoveAction(IUnitActionContext context) : base(context, UnitActionType.Move)
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
            Context.ApplyMovement(Movement.MovementRequest.PathfindToTarget(target.transform, minDistance, maxDistance));

            // Move action is a one-frame command; movement continues in NavMeshAgent.
            Complete();
        }

        private void GetMoveDistanceRange(Unit target, out float minDistance, out float maxDistance)
        {
            switch (Context.CurrentMoveBehaviorMode)
            {
                case Unit.MoveBehaviorMode.TowardAlly:
                    // 靠近队友时尽量贴近，方便保护与协同。
                    minDistance = 0f;
                    maxDistance = Context.BodyRadius + target.BodyRadius + AllyProtectDistancePadding;
                    break;
                case Unit.MoveBehaviorMode.TowardEnemy:
                default:
                    maxDistance = Context.AttackRange;
                    minDistance = maxDistance * HitAndRunMinRatio;
                    break;
            }
        }

        private bool TryGetMoveTarget(out Unit target)
        {
            switch (Context.CurrentMoveBehaviorMode)
            {
                case Unit.MoveBehaviorMode.TowardAlly:
                    if (Context.TryGetClosestAlly(out target))
                    {
                        return true;
                    }

                    return Context.TryGetClosestEnemy(out target);
                case Unit.MoveBehaviorMode.TowardEnemy:
                default:
                    return Context.TryGetClosestEnemy(out target);
            }
        }
    }
}
