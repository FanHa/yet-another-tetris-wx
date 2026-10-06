using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class ProjectileHail : ActiveSkill
    {
        public float damageReductionPercentage = 50f; // 重复攻击时伤害降低百分比
        public float multiplier = 2f; // 攻击频率的倍数
        private readonly List<Unit> cachedTargets = new();

        public override string Name()
        {
            return "弹幕";
        }

        public override string Description()
        {
            return $"向攻击范围内的敌人射出攻击频率 * {multiplier} 的投射物。" +
                   $"优先攻击不重复的敌人，重复攻击时伤害降低 {damageReductionPercentage}%。";
        }

        protected override bool PrepareCastCore()
        {
            cachedTargets.Clear();
            float attackRange = Owner.Attributes.AttackRange.finalValue;
            cachedTargets.AddRange(Owner.FindEnemiesInRange(attackRange));
            return cachedTargets.Count > 0;
        }

        protected override void ExecuteCore()
        {
            if (cachedTargets.Count == 0)
            {
                return;
            }

            float attackFrequency = Owner.Attributes.AttacksPerTenSeconds.finalValue;
            int projectileCount = Mathf.CeilToInt(attackFrequency * multiplier);
            HashSet<Unit> attackedEnemies = new HashSet<Unit>();
            for (int i = 0; i < projectileCount; i++)
            {
                Unit targetEnemy = cachedTargets
                    .FirstOrDefault(enemy => !attackedEnemies.Contains(enemy)) ?? cachedTargets[Random.Range(0, cachedTargets.Count)];

                float damageValue = Owner.Attributes.AttackPower.finalValue;
                if (attackedEnemies.Contains(targetEnemy))
                {
                    damageValue *= 1 - damageReductionPercentage / 100f;
                }
                var damage = new Units.Damages.Damage(damageValue, Units.Damages.DamageType.Hit);
                damage.SetSourceLabel(Name());
                damage.SetSourceUnit(Owner.SelfUnit);
                damage.SetTargetUnit(targetEnemy);
                Owner.SelfUnit.TriggerAttackHit(targetEnemy, damage);
                targetEnemy.TakeHit(Owner.SelfUnit, ref damage);

                // 记录已攻击的敌人
                attackedEnemies.Add(targetEnemy);
            }

            cachedTargets.Clear();
        }

    }
}