using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class PrecisionShoot : Skill
    {
        public string skillName => "精准射击"; // 技能名称
        public float attackPowerMultiplier = 4f; // 攻击力倍数
        public override float cooldown => 10f;
        public float speed = 2f;


        public override void Execute(Unit caster)
        {
            // 找到射程范围内的所有敌人
            Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, caster.attackRange);
            var enemiesInRange = colliders
                .Select(collider => collider.GetComponent<Unit>())
                .Where(enemy => enemy != null && enemy.faction != caster.faction) // 确保是敌人
                .OrderBy(enemy => enemy.maxCore.finalValue) // 按 MaxCore 排序
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for PrecisionShoot.");
                return;
            }

            // 找到 MaxCore 最小的敌人
            Unit targetEnemy = enemiesInRange.First();

             // 创建炸弹实例
            GameObject projectileInstance = Object.Instantiate(caster.PrecisionArrowPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            var projectile = projectileInstance.GetComponent<Units.Projectiles.Projectile>();
            if (projectile != null)
            {
                projectile.target = targetEnemy.transform; // 创建目标位置
                projectile.damage = new Damages.Damage(caster.attackPower.finalValue * attackPowerMultiplier, "精准射击", false); // 设置炸弹伤害
                projectile.speed = speed;
            }
            lastUsedTime = Time.time; // 更新上次使用时间
        }

        public override string Description()
        {
            return $"对射程范围内生命值最低的敌人发射精准箭矢，" +
                $"造成攻击力的 {attackPowerMultiplier} 倍伤害。" +
                $"技能冷却时间为 {cooldown} 秒。";
        }
    }
}