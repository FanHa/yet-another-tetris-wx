using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class BloodBomb : Skill
    {
        public override float cooldown => 15f; // 技能冷却时间
        public float healthPercentage = 20f; // 消耗自身血量的百分比
        public float explosionRadius = 0.75f; // 爆炸范围
        public float healthReturnPercentage = 25f; // 每命中一个敌人返还的血量百分比
        public float speed = 1.5f;

        public override string Name()
        {
            return "血爆弹";
        }

        public override string Description()
        {
            return $"消耗自身当前血量的 {healthPercentage}% 制作成炸弹，随机投向攻击范围内的一个敌人位置。" +
                   $"对范围 {explosionRadius} 内的敌人造成等同于消耗血量的伤害。" +
                   $"每命中一个敌人，返还造成伤害的 {healthReturnPercentage}% 的血量。" +
                   $"技能冷却时间为 {cooldown} 秒。";
        }

        protected override void ExecuteCore(Unit caster)
        {
            if (caster.Attributes.CurrentHealth <= 1)
            {
                Debug.LogWarning("Not enough health to cast BloodBomb.");
                return;
            }

            // 计算消耗的血量
            float healthToConsume = caster.Attributes.CurrentHealth * (healthPercentage / 100f);
            caster.Attributes.CurrentHealth -= healthToConsume;

            // 寻找范围内的敌人
            List<Unit> enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange);

            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for BloodBomb.");
                return;
            }

            // 随机选择一个敌人作为目标
            Unit targetEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];

            // 创建炸弹并投掷到目标位置
            GameObject bombInstance = Object.Instantiate(caster.ProjectileConfig.BloodBombPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            var bloodbomb = bombInstance.GetComponent<Units.Projectiles.BloodBomb>();
            if (bloodbomb != null)
            {
                var damage = new Units.Damages.Damage(healthToConsume, Units.Damages.DamageType.Skill);
                damage.SetSourceLabel(Name());
                damage.SetSourceUnit(caster);

                GameObject tempTargetInstance = Object.Instantiate(caster.ProjectileConfig.TempTargetPrefab);
                tempTargetInstance.transform.position = targetEnemy.transform.position;
                
                bloodbomb.Init(caster, tempTargetInstance.transform, speed, damage, targetEnemy.faction, explosionRadius, healthReturnPercentage);
            }
        }
    }
}