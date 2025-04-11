using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class PrecisionShoot : Skill
    {
        public float attackPowerMultiplier = 4f; // 攻击力倍数
        public override float cooldown => 10f;
        public float speed = 2f;

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.attackRange)
                .OrderBy(enemy => enemy.maxCore.finalValue)
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for PrecisionShoot.");
                return;
            }

            Unit targetEnemy = enemiesInRange.First();
            GameObject projectileInstance = Object.Instantiate(caster.PrecisionArrowPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            var projectile = projectileInstance.GetComponent<Units.Projectiles.Projectile>();
            if (projectile != null)
            {
                projectile.target = targetEnemy.transform;
                projectile.damage = new Damages.Damage(caster.attackPower.finalValue * attackPowerMultiplier, Name(), false);
                projectile.speed = speed;
                projectile.caster = caster;
            }
        }

        public override string Description()
        {
            return $"对射程范围内生命值最低的敌人发射精准箭矢，" +
                $"造成攻击力的 {attackPowerMultiplier} 倍伤害。" +
                $"技能冷却时间为 {cooldown} 秒。";
        }

        public override string Name()
        {
            return "精准打击"; // 返回技能名称
        }
    }
}