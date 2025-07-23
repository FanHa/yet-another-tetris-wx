using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class PrecisionShoot : ActiveSkill
    {
        public float attackPowerMultiplier = 4f; // 攻击力倍数
        public float speed = 1.5f;

        protected override bool ExecuteCore(Unit caster)
        {
            var enemiesInRange = caster.UnitManager.FindEnemiesInRange(caster, caster.Attributes.AttackRange)
                .OrderBy(enemy => enemy.Attributes.MaxHealth.finalValue)
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for PrecisionShoot.");
                return false;
            }

            Unit targetEnemy = enemiesInRange.First();
            GameObject projectileInstance = Object.Instantiate(caster.ProjectileConfig.PrecisionArrowPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            Units.Projectiles.Projectile projectile = projectileInstance.GetComponent<Units.Projectiles.Projectile>();
            if (projectile != null)
            {
                var damage = new Damages.Damage(caster.Attributes.AttackPower.finalValue * attackPowerMultiplier, Damages.DamageType.Hit);
                damage.SetSourceLabel(Name());
                damage.SetSourceUnit(caster);
                damage.SetTargetUnit(targetEnemy);

                var target = targetEnemy.transform;
                projectile.Init(caster, target, damage);
                projectile.Activate();
            }
            return true;
        }

        public override string Description()
        {
            return $"对射程范围内生命值最低的敌人发射精准箭矢，" +
                $"造成攻击力的 {attackPowerMultiplier} 倍伤害。";
        }

        public override string Name()
        {
            return "精准打击"; // 返回技能名称
        }
    }
}