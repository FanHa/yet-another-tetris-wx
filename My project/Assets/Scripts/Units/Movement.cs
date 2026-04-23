using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Units.Unit;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float moveToTargetSampleRadius = 1.5f;
        private Attributes attributes;
        private Controller.UnitManager unitManager; // 新增：由外部注入
        private Unit owner;
        private NavMeshAgent agent;
        private Vector3 lastDestination;

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

        void Awake() // 或 Start
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            
        }

        public void Initialize(Attributes attributes, Controller.UnitManager unitManager, Unit owner)
        {
            this.attributes = attributes;
            this.unitManager = unitManager;
            this.owner = owner;
            agent.enabled = true;

        }

        public MovementResult MoveAlongPathToTarget(Transform targetEnemy)
        {
            if (targetEnemy == null)
            {
                return new MovementResult(MovementResultCode.InvalidRequest, transform.position, transform.position, true);
            }

            Vector3 destination = targetEnemy.position;
            
            // 应用当前的 MoveSpeed 属性（包括 buff 修饰）
            agent.speed = attributes.MoveSpeed.finalValue;

            bool accepted = agent.SetDestination(destination);
            if (!accepted)
            {
                return new MovementResult(MovementResultCode.ExecutionFailed, destination, transform.position, true);
            }

            lastDestination = destination;
            return new MovementResult(MovementResultCode.Success, destination, transform.position, false);
        }

        public MovementResult MoveToDistanceFromTarget(Transform targetEnemy, float minTargetDistance, float maxTargetDistance)
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

            if (Vector3.Distance(destination, lastDestination) <= 0.5f)
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

            lastDestination = destination;
            return new MovementResult(MovementResultCode.Success, destination, transform.position, false);
        }

        public MovementResult MoveStraightByDelta(Vector3 delta)
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
                lastDestination = movedPosition;

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
                lastDestination = agent.nextPosition;

                if (moveDelta.magnitude + 0.001f < requestedDistance)
                {
                    return new MovementResult(MovementResultCode.PartialSuccess, targetPosition, agent.nextPosition, true);
                }

                return new MovementResult(MovementResultCode.Success, targetPosition, agent.nextPosition, false);
            }

            return new MovementResult(MovementResultCode.NoReachablePoint, targetPosition, currentPosition, true);
        }

        public void PlaceAt(Vector3 position)
        {
            // spawn 时 agent 尚未启用，直接设置 transform.position
            // (agent.Warp 在 agent.enabled=false 时会被忽略)
            if (TryResolveReachablePosition(position, out Vector3 resolvedPosition))
            {
                transform.position = resolvedPosition;
                lastDestination = resolvedPosition;
            }
            else
            {
                transform.position = position;
                lastDestination = position;
            }
        }

        public void Teleport(Vector3 position)
        {
            // 仅用于运行时（agent 已初始化），带 fail-fast 保护
            if (!agent.enabled)
                throw new InvalidOperationException("Movement must be initialized before calling Teleport");
            
            if (TryResolveReachablePosition(position, out Vector3 resolvedPosition))
            {
                agent.Warp(resolvedPosition);
                lastDestination = resolvedPosition;
            }
            else
            {
                agent.Warp(position);
                lastDestination = position;
            }
        }

        public void PauseNavigation()
        {
            agent.isStopped = true;
        }

        public void ResumeNavigation()
        {
            agent.isStopped = false;
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

    }
}