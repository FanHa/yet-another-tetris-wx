using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class WindStrike : Skill
    {
        public override float cooldown => 15f; // 技能冷却时间
        public float orbitRadius = 1f; // 旋转半径
        public float orbitSpeedMultiplier = 5f; // 旋转速度倍数
        public float effectDuration = 10f; // 区域效果持续时间
        public float damageInterval = 1f; // 每隔 x 秒检测一次
        public float damageMultiplier = 4f; // 每次检测的伤害

        public override string Description()
        {
            return "";
        }

        public override string Name()
        {
            return "风刃";
        }

        protected override void ExecuteCore(Unit caster)
        {
            // 找到范围内最近的敌人
            Unit targetEnemy = FindClosestEnemy(caster, caster.Attributes.AttackRange);
            if (targetEnemy == null)
            {
                Debug.LogWarning("No valid targets found within range for OrbitStrike.");
                return;
            }

            // 开始旋转协程
            caster.StartCoroutine(Orbit(caster, targetEnemy));
        }

        private IEnumerator Orbit(Unit caster, Unit targetEnemy)
        {
            float orbitSpeed = caster.Attributes.MoveSpeed.finalValue * orbitSpeedMultiplier; // 计算旋转速度
            Vector3 centerPosition = targetEnemy.transform.position; // 目标敌人位置
            float angle = 0f; // 当前角度
            float orbitDuration = 2 * Mathf.PI * orbitRadius / orbitSpeed; // 计算转圈所需时间

            caster.moveable = false; // 禁止移动

            float elapsedTime = 0f;
            while (elapsedTime < orbitDuration)
            {
                // 计算当前位置
                angle += orbitSpeed * Time.deltaTime / orbitRadius; // 根据速度和半径计算角度增量
                float x = Mathf.Cos(angle) * orbitRadius;
                float y = Mathf.Sin(angle) * orbitRadius;
                caster.transform.position = centerPosition + new Vector3(x, y, 0);

                elapsedTime += Time.deltaTime;
                yield return null; // 等待下一帧
            }

            // 恢复单位行为
            caster.moveable = true;
            CreateAreaEffect(caster, centerPosition);

        }

        private void CreateAreaEffect(Unit caster, Vector3 centerPosition)
        {
            // 获取 WindStrikeAreaEffectPrefab 并实例化
            GameObject areaEffect = Object.Instantiate(
                caster.VisualEffectConfig.WindStrikeAreaEffectPrefab, 
                centerPosition, 
                Quaternion.identity
            );
            // 初始化区域效果
            var areaEffectScript = areaEffect.GetComponent<Units.VisualEffects.WindStrikeArea>();
            if (areaEffectScript != null)
            {
                areaEffectScript.Initialize(
                    caster,
                    effectDuration,
                    damageInterval,
                    damageMultiplier,
                    orbitRadius
                );
            }
        }

        
    }
}