using UnityEngine;

namespace Units.Skills
{
    public class Bomb : Skill
    {
        public float damage = 50f; // 炸弹伤害
        public float explosionRadius = 2f; // 爆炸范围
        public override float cooldown => 10f;
        private float speed = 2f;

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange);
            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for Bomb.");
                return;
            }

            Transform targetEnemy = enemiesInRange[0].transform;
            GameObject bombInstance = Object.Instantiate(caster.bombPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            var bomb = bombInstance.GetComponent<Units.Projectiles.Bomb>();
            if (bomb != null)
            {
                bomb.caster = caster;
                bomb.faction = caster.faction;
                bomb.target = new GameObject("BombTarget").transform;
                bomb.target.position = targetEnemy.position;
                bomb.damage = new Damages.Damage(damage, Name(), false);
                bomb.explosionRadius = explosionRadius;
                bomb.speed = speed;
            }
        }

        public override string Description()
        {
            return $"投掷一枚炸弹，对目标区域内的敌人造成 {damage} 点伤害，" +
                $"爆炸范围为 {explosionRadius} 米。" +
                $"技能冷却时间为 {cooldown} 秒。";
        }

        public override string Name()
        {
            return "爆破";
        }
    }
}