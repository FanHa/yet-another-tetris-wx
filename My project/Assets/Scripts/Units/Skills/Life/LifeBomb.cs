using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifeBomb : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeBomb;
        public LifeBombConfig Config { get; }
        private GameObject cachedTempTarget;

        public LifeBomb(LifeBombConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        private struct LifeBombStats
        {
            public StatValue HealthCostPercent;
        }

        private LifeBombStats CalcStats()
        {
            return new LifeBombStats
            {
                HealthCostPercent = new StatValue("消耗生命值百分比", Config.HealthCostPercent)
            };
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;
            var targetEnemy = Owner.UnitManager.FindRandomEnemyInRange(Owner, Owner.Attributes.AttackRange);
            if (targetEnemy == null)
                return false;
            cachedTempTarget = Object.Instantiate(
                Owner.ProjectileConfig.TemporaryTargetPrefab,
                targetEnemy.transform.position,
                Quaternion.identity
            );
            return true;
        }

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();
            float percent = stats.HealthCostPercent.Final / 100f;
            float healthCost = Owner.Attributes.CurrentHealth * percent;
            healthCost = Mathf.Clamp(healthCost, 1f, Owner.Attributes.CurrentHealth); // 至少消耗1点

            // 投射炸弹
            GameObject projectileInstance = Object.Instantiate(
                Owner.ProjectileConfig.LifeBombPrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );
            Units.Projectiles.LifeBomb lifeBomb = projectileInstance.GetComponent<Units.Projectiles.LifeBomb>();

            lifeBomb.Init(
                caster: Owner,
                temporaryTarget: cachedTempTarget,
                healthAmount: healthCost, // 伤害为消耗的生命值
                sourceSkill: this
            );
            cachedTempTarget = null; // 用完清空
            return true;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + "\n" +
                $"{stats.HealthCostPercent}";
        }

        public static string DescriptionStatic() => "消耗自身生命值制作炸弹，对目标区域的敌人造成伤害。";


        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命炸弹";
    }
}