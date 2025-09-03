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
        private Controller.UnitManager unitManager; // 新增：由外部注入
        private Unit owner;
        public bool IsHitAndRun = false;

        public void Initialize(Attributes attributes, Controller.UnitManager unitManager, Unit owner)
        {
            this.attributes = attributes;
            this.unitManager = unitManager;
            this.owner = owner;
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

        public void ExecuteMove(Transform targetEnemy)
        {
            if (targetEnemy == null)
            {
                ResolveCurrentOverlap();
                return;
            }

            float distance = Vector2.Distance(transform.position, targetEnemy.position);

            Vector2 direction;
            if (IsHitAndRun)
            {
                // HitAndRun逻辑：在攻击距离附近徘徊
                float desiredDistance = attributes.AttackRange.finalValue * 0.9f; // 可微调徘徊距离
                if (distance < desiredDistance)
                {
                    // 太近了，远离敌人
                    direction = (transform.position - targetEnemy.position).normalized;
                }
                else if (distance > attributes.AttackRange.finalValue)
                {
                    // 太远了，靠近敌人
                    direction = (targetEnemy.position - transform.position).normalized;
                }
                else
                {
                    // 距离合适，随机微调方向或原地徘徊
                    direction = Vector2.zero;
                }
            }
            else
            {
                // 原计划：靠近敌人
                if (distance <= attributes.AttackRange.finalValue)
                {
                    ResolveCurrentOverlap();
                    return;
                }
                direction = (targetEnemy.position - transform.position).normalized;
            }

            if (direction != Vector2.zero)
            {
                Vector2 adjustedDirection = AdjustDirectionToAvoidUnits(direction);
                Vector2 newPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + adjustedDirection, attributes.MoveSpeed.finalValue * Time.deltaTime);
                transform.position = newPosition;

                AdjustLookDirection(adjustedDirection);
                ClampPositionToBattlefield();
            }
            else
            {
                // 静止时也要检测并推开重叠
                ResolveCurrentOverlap();
            }
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


            Vector2 avoidanceVector = Vector2.zero;
            int avoidanceCount = 0;

            var allies = unitManager.FindAlliesInRange(owner, minDistance, includeSelf: false);
            var enemies = unitManager.FindEnemiesInRange(owner, minDistance);

            if (allies != null)
            {
                foreach (var other in allies)
                {
                    if (other == null || !other.IsActive) continue;
                    Vector2 toOther = (Vector2)transform.position - (Vector2)other.transform.position;
                    float dist = Mathf.Max(toOther.magnitude, 0.1f);
                    avoidanceVector += toOther.normalized / dist;
                    avoidanceCount++;
                }
            }
            if (enemies != null)
            {
                foreach (var other in enemies)
                {
                    if (other == null || !other.IsActive) continue;
                    Vector2 toOther = (Vector2)transform.position - (Vector2)other.transform.position;
                    float dist = Mathf.Max(toOther.magnitude, 0.1f);
                    avoidanceVector += toOther.normalized / dist;
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
        
        private void ResolveCurrentOverlap()
        {
            var allUnits = unitManager.GetAllUnits();
            foreach (var other in allUnits)
            {
                if (other == owner || !other.IsActive) continue;
                float actualDist = Vector2.Distance(transform.position, other.transform.position);
                if (actualDist < minDistance && actualDist > 0.01f)
                {
                    Vector2 dir = ((Vector2)transform.position - (Vector2)other.transform.position).normalized;
                    float pushDist = minDistance - actualDist;
                    transform.position += (Vector3)(dir * pushDist * 0.5f);
                    other.transform.position -= (Vector3)(dir * pushDist * 0.5f);
                }
            }
        }
    }
}