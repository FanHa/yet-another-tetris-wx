using UnityEngine;

namespace Units.Skills
{
    public class Bomb : Skill
    {
        public float damage = 50f; // 炸弹伤害
        public float explosionRadius = 2f; // 爆炸范围
        private float cooldown = 10f;
        private float speed = 2f;

        public override float Cooldown()
        {
            return cooldown;
        }

        public override void Execute(Unit caster)
        {
            if (caster.targetEnemies == null || caster.targetEnemies.Count == 0)
            {
                Debug.LogWarning("No target enemies to throw bomb at.");
                return;
            }

            // 获取目标敌人的位置（这里选择第一个敌人）
            Transform targetEnemy = caster.targetEnemies[0];
            if (targetEnemy == null)
            {
                Debug.LogWarning("Target enemy is null.");
                return;
            }

            // 创建炸弹实例
            GameObject bombInstance = Object.Instantiate(caster.bombPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            var bomb = bombInstance.GetComponent<Units.Projectiles.Bomb>();
            if (bomb != null)
            {
                bomb.faction = caster.faction; // 设置炸弹的阵营
                bomb.target = new GameObject("BombTarget").transform; // 创建目标位置
                bomb.target.position = targetEnemy.position; // 设置目标位置为敌人位置
                bomb.damage = damage; // 设置炸弹伤害
                bomb.explosionRadius = explosionRadius; // 设置爆炸范围
                bomb.speed = speed; // 设置炸弹速度
            }
            lastUsedTime = Time.time; // 更新上次使用时间
        }
    }
}