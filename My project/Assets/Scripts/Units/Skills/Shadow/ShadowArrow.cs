using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowArrow : ActiveSkill
    {
        public ShadowArrowLevelConfig Config { get; }

        private ShadowArrowSkillConfig SkillConfig => (ShadowArrowSkillConfig)Definition.Config;

        public ShadowArrow(ShadowArrowLevelConfig config)
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
        protected override void ExecuteCore()
        {
            // 查找最大生命值最低的敌人
            Unit targetEnemy = Owner.FindLowestMaxHealthEnemy();
            if (targetEnemy == null)
                return;

            var stats = CalcStats();
            // 创建并发射暗影箭
            var projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );

            projectile.Init(
                caster: Owner.SelfUnit,
                target: targetEnemy,
                vulnerabilityPercent: stats.VulnerabilityPercent.Final,
                vulnerabilityDuration: stats.VulnerabilityDuration.Final,
                sourceSkill: this
            );
            projectile.Activate();
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

        public static string DescriptionStatic() => "向最大生命值最低的敌人发射暗影箭,施加易伤Debuff并造成伤害";

        public override string Name() => NameStatic();
        public static string NameStatic() => "暗影箭";
    }

}