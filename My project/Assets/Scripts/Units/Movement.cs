using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float moveToTargetSampleRadius = 1.5f;
        [SerializeField] private float navigationRetargetDeadZone = 0.5f;
        private Attributes attributes;
        private NavMeshAgent agent;
        private Vector3 lastPathDestination;
        private bool hasLastPathDestination;
        private int? cachedAvoidancePriority;
        public float AgentRadius => agent.radius;

        public enum MovementResultCode
        {
            Success,
            PartialSuccess,
            NoOp,
            InvalidRequest,
            AgentUnavailable,
            NoReachablePoint,
            Blocked,
            ExecutionFailed
        }

        public readonly struct MovementResult
        {
            public MovementResultCode Code { get; }
            public Vector3 RequestedPosition { get; }
            public Vector3 FinalPosition { get; }
            public Vector3 BlockedPosition { get; }
            public bool HasBlockedPosition { get; }
            public bool ShouldTerminate { get; }

            public MovementResult(
                MovementResultCode code,
                Vector3 requestedPosition,
                Vector3 finalPosition,
                bool shouldTerminate,
                bool hasBlockedPosition = false,
                Vector3 blockedPosition = default)
            {
                Code = code;
                RequestedPosition = requestedPosition;
                FinalPosition = finalPosition;
                ShouldTerminate = shouldTerminate;
                HasBlockedPosition = hasBlockedPosition;
                BlockedPosition = blockedPosition;
            }
        }

        // ============ 位移请求模型 ============
        public enum MovementMode
        {
            DirectMove,          // 直线位移
            PathfindToTarget,    // 寻路到目标距离
            Teleport,            // 传送
            PlaceAt              // 初始放置
        }

        public readonly struct MovementRequest
        {
            public MovementMode Mode { get; }
            public Vector3 Delta { get; }
            public Transform Target { get; }
            public float MinDistance { get; }
            public float MaxDistance { get; }
            public Vector3 Position { get; }

            private MovementRequest(
                MovementMode mode,
                Vector3 delta,
                Transform target,
                float minDistance,
                float maxDistance,
                Vector3 position)
            {
                Mode = mode;
                Delta = delta;
                Target = target;
                MinDistance = minDistance;
                MaxDistance = maxDistance;
                Position = position;
            }

            public static MovementRequest DirectMove(Vector3 delta)
            {
                return new MovementRequest(MovementMode.DirectMove, delta, null, 0f, 0f, default);
            }

            public static MovementRequest PathfindToTarget(Transform target, float minDistance, float maxDistance)
            {
                return new MovementRequest(MovementMode.PathfindToTarget, default, target, minDistance, maxDistance, default);
            }

            public static MovementRequest Teleport(Vector3 position)
            {
                return new MovementRequest(MovementMode.Teleport, default, null, 0f, 0f, position);
            }

            public static MovementRequest PlaceAt(Vector3 position)
            {
                return new MovementRequest(MovementMode.PlaceAt, default, null, 0f, 0f, position);
            }
        }

        void Awake() // 或 Start
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            
        }

        public void Initialize(Attributes attributes)
        {
            this.attributes = attributes;
            agent.enabled = true;
        }

        private MovementResult MoveToDistanceFromTarget(Transform targetEnemy, float minTargetDistance, float maxTargetDistance)
        {
            if (targetEnemy == null)
            {
                return new MovementResult(MovementResultCode.InvalidRequest, transform.position, transform.position, true);
            }

            if (minTargetDistance < 0f || maxTargetDistance < minTargetDistance)
            {
                return new MovementResult(MovementResultCode.InvalidRequest, targetEnemy.position, transform.position, true);
            }

            float distance = Vector2.Distance(transform.position, targetEnemy.position);
            Vector3 destination;

            if (distance < minTargetDistance)
            {
                Vector2 away = ((Vector2)transform.position - (Vector2)targetEnemy.position).normalized;
                destination = transform.position + (Vector3)(away * 0.5f);
            }
            else if (distance > maxTargetDistance)
            {
                destination = targetEnemy.position;
            }
            else
            {
                return new MovementResult(MovementResultCode.NoOp, targetEnemy.position, transform.position, false);
            }

            if (hasLastPathDestination && Vector3.Distance(destination, lastPathDestination) <= navigationRetargetDeadZone)
            {
                return new MovementResult(MovementResultCode.NoOp, destination, transform.position, false);
            }

            // 应用当前的 MoveSpeed 属性（包括 buff 修饰）
            agent.speed = attributes.MoveSpeed.finalValue;

            bool accepted = agent.SetDestination(destination);
            if (!accepted)
            {
                return new MovementResult(MovementResultCode.ExecutionFailed, destination, transform.position, true);
            }

            SetLastPathDestination(destination);
            return new MovementResult(MovementResultCode.Success, destination, transform.position, false);
        }

        private MovementResult MoveStraightByDelta(Vector3 delta)
        {
            Vector3 currentPosition = agent.nextPosition;
            float requestedDistance = delta.magnitude;

            Vector3 targetPosition = currentPosition + delta;
            if (agent.Raycast(targetPosition, out NavMeshHit blockedHit))
            {
                Vector3 allowedDelta = blockedHit.position - currentPosition;
                Vector3 raycastDelta = allowedDelta * 0.95f;
                agent.Move(raycastDelta);
                Vector3 movedPosition = agent.nextPosition;

                if ((movedPosition - currentPosition).magnitude <= 0.001f)
                {
                    return new MovementResult(
                        MovementResultCode.Blocked,
                        targetPosition,
                        currentPosition,
                        true,
                        true,
                        blockedHit.position);
                }

                return new MovementResult(
                    MovementResultCode.PartialSuccess,
                    targetPosition,
                    movedPosition,
                    true,
                    true,
                    blockedHit.position);
            }

            if (TryResolveReachablePosition(targetPosition, out Vector3 reachablePosition))
            {
                Vector3 moveDelta = reachablePosition - currentPosition;
                if (moveDelta.magnitude <= 0.001f)
                {
                    return new MovementResult(
                        MovementResultCode.Blocked,
                        targetPosition,
                        currentPosition,
                        true,
                        false,
                        default);
                }

                agent.Move(moveDelta);

                if (moveDelta.magnitude + 0.001f < requestedDistance)
                {
                    return new MovementResult(MovementResultCode.PartialSuccess, targetPosition, agent.nextPosition, true);
                }

                return new MovementResult(MovementResultCode.Success, targetPosition, agent.nextPosition, false);
            }

            return new MovementResult(MovementResultCode.NoReachablePoint, targetPosition, currentPosition, true);
        }

        private void PlaceAt(Vector3 position)
        {
            // spawn 时 agent 尚未启用，直接设置 transform.position
            // (agent.Warp 在 agent.enabled=false 时会被忽略)
            if (TryResolveReachablePosition(position, out Vector3 resolvedPosition))
            {
                transform.position = resolvedPosition;
            }
            else
            {
                transform.position = position;
            }

            InvalidateLastPathDestination();
        }

        private void Teleport(Vector3 position)
        {
            
            if (TryResolveReachablePosition(position, out Vector3 resolvedPosition))
            {
                agent.Warp(resolvedPosition);
            }
            else
            {
                agent.Warp(position);
            }

            InvalidateLastPathDestination();
        }

        internal void PauseNavigation()
        {
            agent.isStopped = true;
        }

        internal void ResumeNavigation()
        {
            agent.isStopped = false;
        }

        internal void ClearNavigationPath()
        {
            agent.ResetPath();
            InvalidateLastPathDestination();
        }

        internal void EnterSkillMotion(int avoidancePriority)
        {

            if (!cachedAvoidancePriority.HasValue)
            {
                cachedAvoidancePriority = agent.avoidancePriority;
            }

            agent.avoidancePriority = Mathf.Clamp(avoidancePriority, 0, 99);
        }

        internal void ExitSkillMotion()
        {

            if (!cachedAvoidancePriority.HasValue)
            {
                return;
            }

            agent.avoidancePriority = cachedAvoidancePriority.Value;
            cachedAvoidancePriority = null;
        }

        private bool TryResolveReachablePosition(Vector3 targetPosition, out Vector3 resolvedPosition)
        {
            // 优先用目标点附近最接近的 NavMesh 点。
            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit sampleHit, moveToTargetSampleRadius, agent.areaMask))
            {
                resolvedPosition = sampleHit.position;
                return true;
            }

            resolvedPosition = default;
            return false;
        }

        private void SetLastPathDestination(Vector3 destination)
        {
            lastPathDestination = destination;
            hasLastPathDestination = true;
        }

        private void InvalidateLastPathDestination()
        {
            hasLastPathDestination = false;
            lastPathDestination = default;
        }

        // ============ 位移请求唯一入口 ============

        /// <summary>
        /// 应用位移操作。根据 MovementMode 执行相应的位移逻辑。所有位移都是同步执行。
        /// </summary>
        public MovementResult ApplyMovement(MovementRequest request)
        {
            switch (request.Mode)
            {
                case MovementMode.DirectMove:
                    return MoveStraightByDelta(request.Delta);

                case MovementMode.PathfindToTarget:
                    return MoveToDistanceFromTarget(request.Target, request.MinDistance, request.MaxDistance);

                case MovementMode.Teleport:
                    Teleport(request.Position);
                    return new MovementResult(
                        MovementResultCode.Success,
                        request.Position,
                        transform.position,
                        false);

                case MovementMode.PlaceAt:
                    PlaceAt(request.Position);
                    return new MovementResult(
                        MovementResultCode.Success,
                        request.Position,
                        transform.position,
                        false);

                default:
                    return new MovementResult(
                        MovementResultCode.InvalidRequest,
                        Vector3.zero,
                        transform.position,
                        true);
            }
        }

    }
}