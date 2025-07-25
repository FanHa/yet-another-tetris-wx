using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class ChainLightning : ActiveSkill
    {
        public float baseDamage = 20f; // 初始伤害
        public float damageIncreasePercentage = 20f; // 每次弹射伤害增加百分比
        public int maxBounces = 5; // 最大弹射次数
        public float range = 5f; // 闪电作用范围

        public override string Description()
        {
            return $"对最近的敌人发射一道闪电，造成 {baseDamage} 点伤害，" +
                   $"每次弹射伤害增加 {damageIncreasePercentage}%，最多弹射 {maxBounces} 次。";
        }

        protected override bool ExecuteCore(Unit caster)
        {
            // 找到范围内最近的敌人
            Unit initialTarget = caster.UnitManager.FindClosestEnemyInRange(caster, range);
            if (initialTarget == null)
            {
                Debug.LogWarning("No valid targets found within range for ChainLightning.");
                return false;
            }

            // 开始闪电弹射逻辑
            caster.StartCoroutine(ChainLightningRoutine(caster, initialTarget));
            return true;
        }

        private System.Collections.IEnumerator ChainLightningRoutine(Unit caster, Unit initialTarget)
        {
            Unit currentTarget = initialTarget;
            float currentDamage = baseDamage;
            HashSet<Unit> hitTargets = new HashSet<Unit>(); // 记录已命中的敌人
            int bounces = 0;

            Vector3 previousPosition = caster.transform.position; // 初始起点为 caster 的位置


            while (currentTarget != null && bounces < maxBounces)
            {
                var damage = new Units.Damages.Damage(currentDamage, Damages.DamageType.Skill);
                damage.SetSourceLabel(Name());
                damage.SetSourceUnit(caster);
                damage.SetTargetUnit(currentTarget);
                currentTarget.TakeDamage(damage);


                // 创建 ChainLightningController 实例并设置起点和终点
                if (caster.ProjectileConfig.ChainLightningPrefab != null)
                {
                    var lightningInstance = Object.Instantiate(caster.ProjectileConfig.ChainLightningPrefab, caster.transform.position, Quaternion.identity);
                    var controller = lightningInstance.GetComponent<Projectiles.ChainLightning>();
                    if (controller != null)
                    {
                        controller.SetLinePoints(previousPosition, currentTarget.transform.position);
                    }
                }

                previousPosition = currentTarget.transform.position;
                // 记录已命中的目标
                hitTargets.Add(currentTarget);

                // 查找下一个最近的敌人
                Unit nextTarget = caster.UnitManager.FindEnemiesInRange(caster, range)
                    .Where(enemy => !hitTargets.Contains(enemy)) // 排除已命中的敌人
                    .OrderBy(enemy => (currentTarget.transform.position - enemy.transform.position).sqrMagnitude)
                    .FirstOrDefault();

                // 等待一小段时间模拟弹射延迟
                yield return new WaitForSeconds(0.2f);

                // 更新目标和伤害
                currentTarget = nextTarget;
                currentDamage += currentDamage * (damageIncreasePercentage / 100f);
                bounces++;
            }
        }

        public override string Name()
        {
            return "闪电链";
        }
    }
}