using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FlameRing : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameRing;

        public FlameRingConfig Config { get; }

        public FlameRing(FlameRingConfig config)
        {
            Config = config;
        }

        private struct FlameRingStats
        {
            public StatValue DotDps;
            public StatValue DotDuration;
            public StatValue Radius;
            public StatValue BuffDuration;
        }

        private FlameRingStats CalcStats()
        {
            int fireCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            return new FlameRingStats
            {
                DotDps = new StatValue("每秒火焰伤害", Config.BaseDotDps, fireCellCount * Config.DotDpsPerFireCell),
                DotDuration = new StatValue("火焰伤害持续时间", Config.BaseDotDuration, fireCellCount * Config.DotDurationPerFireCell),
                Radius = new StatValue("作用半径", Config.BaseRadius, fireCellCount * Config.RadiusPerFireCell),
                BuffDuration = new StatValue("效果持续时间", Config.BuffDuration)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.DotDps}\n" +
                $"{stats.DotDuration}\n" +
                $"{stats.Radius}\n";
        }

        public static string DescriptionStatic() => "对周围一圈敌人持续造成火焰伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "火环";

        public void ApplyPassive()
        {
            var stats = CalcStats();
            var buff = new Buffs.FlameRing(
                stats.DotDps.Final,
                stats.DotDuration.Final,
                stats.BuffDuration.Final,
                stats.Radius.Final,
                Owner,
                this
            );
            Owner.AddBuff(buff);
        }
    }
}