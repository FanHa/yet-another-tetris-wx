using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class Fireball : Skill
    {
        public float BaseDamage = 20f;
        public float FireCellDamageBonus = 5f;
        public float DotPerFireCell = 1f;           // 每个火系Cell dot伤害
        public float DotDurationPerFireCell = 1f; // 每个火系Cell增加的dot持续时间

        public float DotBaseDuration = 3f;              // dot持续时间

        private int fireCellCount = 0;

        public Fireball()
        {
            RequiredEnergy = 100f; // 设置技能所需能量
        }

        public void SetFireCellCount(int fireCellCount)
        {
            this.fireCellCount = fireCellCount;
        }

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange)
                .OrderBy(enemy => Vector3.Distance(caster.transform.position, enemy.transform.position))
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                return;
            }

            Unit targetEnemy = enemiesInRange.First();
            GameObject projectileInstance = Object.Instantiate(caster.ProjectileConfig.FireballPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            Units.Projectiles.Fireball fireball = projectileInstance.GetComponent<Units.Projectiles.Fireball>();
            if (fireball != null)
            {
                float totalDamage = BaseDamage + fireCellCount * FireCellDamageBonus;
                var damage = new Damages.Damage(totalDamage, Damages.DamageType.Skill);
                damage.SetSourceLabel(Name());
                damage.SetSourceUnit(caster);
                damage.SetTargetUnit(targetEnemy);

                var target = targetEnemy.transform;
                var dot = new Units.Dot(
                    Units.DotType.Burn,
                    this,
                    caster,
                    fireCellCount * DotPerFireCell,    // 伤害值为 fireCellCount
                    DotBaseDuration + fireCellCount * DotDurationPerFireCell,
                    "灼烧"
                );
                fireball.SetDot(dot);
                fireball.Init(caster, target, damage);
            }
        }

        public override string Description()
        {
            return $"向攻击范围内一个敌人发射火球，造成 {BaseDamage} 点火焰伤害";
        }

        public override string Name()
        {
            return "火球";
        }
    }
}