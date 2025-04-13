using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class Repel : Skill
    {
        public override float cooldown => 10f;
        public float repelDistance = 3f; // 击退距离
        public float repelDuration = 0.5f; // 击退持续时间

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange);
            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for Repel.");
                return;
            }

            Transform targetEnemy = enemiesInRange[0].transform;
            caster.StartCoroutine(RepelTargetEnemy(caster, targetEnemy));
        }

        private IEnumerator RepelTargetEnemy(Unit caster, Transform targetEnemy)
        {
            // 获取目标的 Unit 组件
            Unit targetUnit = targetEnemy.GetComponent<Unit>();
            if (targetUnit == null)
            {
                Debug.LogWarning("Target does not have a Unit component.");
                yield break;
            }

            // 禁用目标的移动能力
            targetUnit.moveable = false;
            Vector2 startPosition = targetEnemy.position;
            Vector2 repelDirection = (targetEnemy.position - caster.transform.position).normalized; // 修正方向计算
            Vector2 targetPosition = (Vector2)targetEnemy.position + repelDirection * repelDistance;

            float elapsedTime = 0f; // 已经过的时间

            while (elapsedTime < repelDuration)
            {
                // 计算目标敌人的下一步位置
                Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / repelDuration);
                targetEnemy.position = newPosition;

                // 更新已经过的时间
                elapsedTime += Time.deltaTime;

                yield return null; // 等待下一帧
            }

            // 将目标敌人移动到最终位置
            targetEnemy.position = targetPosition;
            targetUnit.moveable = true;

        }

        public override string Description()
        {
            return $"对目标敌人施加击退效果，将其击退 {repelDistance} 米，" +
                $"击退过程持续 {repelDuration} 秒。" +
                $"技能冷却时间为 {cooldown} 秒。";
        }

        public override string Name()
        {
            return "击退";
        }
    }
}