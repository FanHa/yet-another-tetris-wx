using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowArrow : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowArrow;
        public ShadowArrowConfig Config { get; }

        public ShadowArrow(ShadowArrowConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }
        
        private struct ShadowArrowStats
        {
            public StatValue Damage;
            public StatValue VulnerabilityPercent;
            public StatValue VulnerabilityDuration;
        }

        private ShadowArrowStats CalcStats()
        {
            int shadowCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Shadow, out var count) ? count : 0;
            return new ShadowArrowStats
            {
                Damage = new StatValue("伤害", Config.Damage, shadowCellCount * Config.DamagePerShadowCell),
                VulnerabilityPercent = new StatValue("易伤百分比(%)", Config.VulnerabilityPercent, shadowCellCount * Config.VulnerabilityPercentPerShadowCell),
                VulnerabilityDuration = new StatValue("易伤持续时间", Config.VulnerabilityDuration)
            };
        }
        protected override bool ExecuteCore()
        {
            // 查找最弱敌人
            Unit targetEnemy = Owner.UnitManager.FindWeakestEnemy(Owner);
            if (targetEnemy == null)
                return false;

            var stats = CalcStats();
            // 创建并发射暗影箭
            var projectilePrefab = Owner.ProjectileConfig.ShadowArrowPrefab;
            var projectile = Object.Instantiate(projectilePrefab, Owner.transform.position, Quaternion.identity)
                .GetComponent<Units.Projectiles.ShadowArrow>();

            projectile.Init(
                caster: Owner,
                target: targetEnemy,
                vulnerabilityPercent: stats.VulnerabilityPercent.Final,
                vulnerabilityDuration: stats.VulnerabilityDuration.Final,
                sourceSkill: this
            );
            projectile.Activate();
            return true;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Damage}\n" +
                $"{stats.VulnerabilityPercent}\n" +
                $"{stats.VulnerabilityDuration}\n";
        }

        public static string DescriptionStatic() => "向脆弱的敌人发射暗影箭,施加易伤Debuff并造成伤害";

        public override string Name() => NameStatic();
        public static string NameStatic() => "暗影箭";
    }

}