using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class Bomb : Skill
    {
        protected float damageValue = 50f; // 炸弹伤害
        public float explosionRadius = 0.75f; // 爆炸范围
        public override float cooldown => 10f;
        private float speed = 1.5f;

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange);
            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for Bomb.");
                return;
            }

            Unit targetEnemy = enemiesInRange[0];
            GameObject bombInstance = Object.Instantiate(caster.ProjectileConfig.BombPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            var bomb = bombInstance.GetComponent<Units.Projectiles.Bomb>();
            if (bomb != null)
            {
                Damages.Damage damage = new Damages.Damage(
                    damageValue, 
                    Name(),
                    Damages.DamageType.Skill,
                    caster,
                    null,
                    new List<Buffs.Buff>()
                );

                GameObject tempTargetInstance = Object.Instantiate(caster.ProjectileConfig.TempTargetPrefab);
                tempTargetInstance.transform.position = targetEnemy.transform.position;
                
                bomb.Init(caster, tempTargetInstance.transform, speed, damage, targetEnemy.faction, explosionRadius);
            }
        }

        public override string Description()
        {
            return $"投掷一枚炸弹，对目标区域内的敌人造成 {damageValue} 点伤害，" +
                $"爆炸范围为 {explosionRadius} 米。" +
                $"技能冷却时间为 {cooldown} 秒。";
        }

        public override string Name()
        {
            return "爆破";
        }
    }
}