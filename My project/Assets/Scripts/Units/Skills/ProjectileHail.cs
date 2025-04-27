using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class ProjectileHail : Skill
    {
        public override float cooldown => 12f; // 技能冷却时间
        public float damageReductionPercentage = 50f; // 重复攻击时伤害降低百分比
        public float multiplier = 2f; // 攻击频率的倍数

        public override string Name()
        {
            return "弹幕";
        }

        public override string Description()
        {
            return $"向攻击范围内的敌人射出攻击频率 * {multiplier} 的投射物。" +
                   $"优先攻击不重复的敌人，重复攻击时伤害降低 {damageReductionPercentage}%。" +
                   $"技能冷却时间为 {cooldown} 秒。";
        }

        protected override void ExecuteCore(Unit caster)
        {
            float attackRange = caster.Attributes.AttackRange;
            float attackFrequency = caster.Attributes.AttacksPerTenSeconds.finalValue;
            int projectileCount = Mathf.CeilToInt(attackFrequency * multiplier);

            List<Unit> enemiesInRange = FindEnemiesInRange(caster, attackRange);
            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for ProjectileHail.");
                return;
            }

            HashSet<Unit> attackedEnemies = new HashSet<Unit>();
            for (int i = 0; i < projectileCount; i++)
            {
                Unit targetEnemy = enemiesInRange
                    .FirstOrDefault(enemy => !attackedEnemies.Contains(enemy)) ?? enemiesInRange[Random.Range(0, enemiesInRange.Count)];

                float damageValue = caster.Attributes.AttackPower.finalValue;
                if (attackedEnemies.Contains(targetEnemy))
                {
                    damageValue *= 1 - damageReductionPercentage / 100f;
                }

                Damages.Damage damage = new Units.Damages.Damage(
                    damageValue,
                    Name(),
                    Units.Damages.DamageType.Hit,
                    caster,
                    targetEnemy,
                    caster.attackEffects
                );
                caster.FireProjectile(targetEnemy, damage);

                // 记录已攻击的敌人
                attackedEnemies.Add(targetEnemy);
            }
        }

    }
}