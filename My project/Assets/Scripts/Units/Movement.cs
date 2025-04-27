using System.Collections.Generic;
using UnityEngine;
using static Units.Unit;

namespace Units
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float minDistance; // 与其他单位保持的最小距离
        public Transform BattlefieldMinBounds;
        public Transform BattlefieldMaxBounds;
        private Attributes attributes;

        public void Initialize(Attributes attributes)
        {
            this.attributes = attributes;
        }

        public void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
        {
            BattlefieldMinBounds = minBounds;
            BattlefieldMaxBounds = maxBounds;
        }

        public (Vector2 minBounds, Vector2 maxBounds) GetBattlefieldBounds()
        {
            if (BattlefieldMinBounds == null || BattlefieldMaxBounds == null)
            {
                Debug.LogWarning("Battlefield bounds are not set.");
                return (Vector2.zero, Vector2.zero); // 返回默认值
            }

            Vector2 minBounds = BattlefieldMinBounds.position;
            Vector2 maxBounds = BattlefieldMaxBounds.position;

            return (minBounds, maxBounds);
        }

        public void MoveTowardsEnemy(Transform closestEnemy)
        {
            if (closestEnemy == null) return;

            float distance = Vector2.Distance(transform.position, closestEnemy.position);
            if (distance <= attributes.AttackRange)
            {
                return;
            }
            // 计算移动方向
            Vector2 direction = (closestEnemy.position - transform.position).normalized;

            // 调整方向以避免扎堆
            Vector2 adjustedDirection = AdjustDirectionToAvoidUnits(direction);

            // 移动单位
            Vector2 newPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + adjustedDirection, attributes.MoveSpeed.finalValue * Time.deltaTime);
            transform.position = newPosition;

            // 调整朝向
            AdjustLookDirection(adjustedDirection);
            ClampPositionToBattlefield();
        }

        private void ClampPositionToBattlefield()
        {
            if (BattlefieldMinBounds == null || BattlefieldMaxBounds == null)
            {
                Debug.LogWarning("Battlefield bounds are not set.");
                return;
            }

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, BattlefieldMinBounds.position.x, BattlefieldMaxBounds.position.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, BattlefieldMinBounds.position.y, BattlefieldMaxBounds.position.y);
            transform.position = clampedPosition;
        }

        private Vector2 AdjustDirectionToAvoidUnits(Vector2 originalDirection)
        {
            // 获取附近的单位
            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, minDistance);

            Vector2 avoidanceVector = Vector2.zero;
            int avoidanceCount = 0;

            foreach (var collider in nearbyUnits)
            {
                if (collider.gameObject != gameObject && collider.TryGetComponent<Unit>(out Unit otherUnit))
                {
                    Vector2 toOtherUnit = (Vector2)(transform.position - otherUnit.transform.position);
                    float distance = toOtherUnit.magnitude;

                    // 根据距离的倒数加权，距离越近影响越大
                    avoidanceVector += toOtherUnit.normalized / Mathf.Max(distance, 0.1f);
                    avoidanceCount++;
                }
            }

            if (avoidanceCount > 0)
            {
                // 平均避让方向
                avoidanceVector /= avoidanceCount;

                // 平衡目标方向和避让方向的权重
                float targetWeight = 0.7f; // 目标方向的权重
                float avoidanceWeight = 0.3f; // 避让方向的权重

                return (originalDirection * targetWeight + avoidanceVector * avoidanceWeight).normalized;
            }

            // 如果没有需要避让的单位，返回原始方向
            return originalDirection;
        }

        private void AdjustLookDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 计算角度
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // 设置自身旋转
        }
    }
}