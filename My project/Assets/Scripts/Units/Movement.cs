using System.Collections.Generic;
using UnityEngine;
using static Units.Unit;

namespace Units
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float minDistance; // 与敌人保持的最小距离
        private Transform battlefieldMinBounds;
        private Transform battlefieldMaxBounds;
        private Attributes attributes;

        public void Initialize(Attributes attributes)
        {
            this.attributes = attributes;
        }

        public void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
        {
            battlefieldMinBounds = minBounds;
            battlefieldMaxBounds = maxBounds;
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
            if (battlefieldMinBounds == null || battlefieldMaxBounds == null)
            {
                Debug.LogWarning("Battlefield bounds are not set.");
                return;
            }

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, battlefieldMinBounds.position.x, battlefieldMaxBounds.position.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, battlefieldMinBounds.position.y, battlefieldMaxBounds.position.y);
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
                    // 计算避让方向
                    Vector2 toOtherUnit = (Vector2)(transform.position - otherUnit.transform.position);
                    avoidanceVector += toOtherUnit.normalized;
                    avoidanceCount++;
                }
            }

            if (avoidanceCount > 0)
            {
                // 平均避让方向
                avoidanceVector /= avoidanceCount;

                // 调整原始方向
                return (originalDirection + avoidanceVector).normalized;
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