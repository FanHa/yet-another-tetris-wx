using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowAttack : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowAttack;
        public ShadowAttackConfig Config { get; }

        public ShadowAttack(ShadowAttackConfig config)
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
            int shadowCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Shadow, out var count) ? count : 0;
            return new ShadowAttackStats
            {
                VulnerabilityPercent = new StatValue("易伤百分比", Config.VulnerabilityPercent, shadowCellCount * Config.VulnerabilityPercentPerShadowCell),
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

        public void ApplyPassive()
        {
            var stats = CalcStats();
            var buff = new Buffs.ShadowAttackBuff(
                stats.VulnerabilityPercent.Final,
                stats.DotDuration.Final,
                Owner,
                this
            );
            Owner.AddBuff(buff);
        }
    }
}