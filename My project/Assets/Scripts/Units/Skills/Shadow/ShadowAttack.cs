using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowAttack : Skill
    {
        public ShadowAttackLevelConfig Config { get; }

        public ShadowAttack(ShadowAttackLevelConfig config)
        {
            Config = config;
        }

        private struct ShadowAttackStats
        {
            public StatValue VulnerabilityPercent;
            public StatValue DotDuration;
        }

        private ShadowAttackStats CalcStats()
        {
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new ShadowAttackStats
            {
                VulnerabilityPercent = new StatValue("易伤百分比", Config.VulnerabilityPercent, resonantCellCount * Config.VulnerabilityPercentPerResonantCell),
                DotDuration = new StatValue("易伤持续时间", Config.DotDuration)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.VulnerabilityPercent}\n" +
                $"{stats.DotDuration}\n";
        }
        public static string DescriptionStatic() => "攻击时附加易伤";

        public override string Name() => NameStatic();
        public static string NameStatic() => "影袭";

        public override void OnApplyPassive()
        {
            var stats = CalcStats();
            var config = (ShadowAttackSkillConfig)Definition.Config;
            var buff = Buffs.BuffFactory.Create(
                config.ShadowAttackBuffDefinition,
                new Buffs.ShadowAttackBuffArgs(
                    config.VulnerabilityBuffDefinition,
                    stats.VulnerabilityPercent.Final,
                    stats.DotDuration.Final,
                    new Buffs.BuffSource(Owner.SelfUnit, this))
            );
            Owner.AddBuff(buff);
        }
    }
}