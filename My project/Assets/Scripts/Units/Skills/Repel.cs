using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class Repel : Skill
    {
        public string skillName = "击退"; // 技能名称
        public override float cooldown => 10f;
        public float repelDistance = 3f; // 击退距离
        public float repelDuration = 0.5f; // 击退持续时间

        public override void Execute(Unit caster)
        {
            if (caster.targetEnemies == null || caster.targetEnemies.Count == 0)
            {
                Debug.LogWarning("No target enemies to rush towards.");
                return;
            }

            // 获取目标敌人的位置（这里选择第一个敌人）
            Transform targetEnemy = caster.targetEnemies[0];
            if (targetEnemy == null)
            {
                Debug.LogWarning("Target enemy is null.");
                return;
            }

            // 开始冲刺协程
            caster.StartCoroutine(RepelTargetEnemy(caster, targetEnemy));
            lastUsedTime = Time.time; // 更新上次使用时间
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
    }
}