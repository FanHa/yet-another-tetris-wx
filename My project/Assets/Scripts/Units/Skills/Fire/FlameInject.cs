using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FlameInject : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameInject;
        public FlameInjectConfig Config { get; }

        public FlameInject(FlameInjectConfig config)
        {
            Config = config;
        }

        private struct FlameInjectStats
        {
            public StatValue DotDps;
            public StatValue DotDuration;
            public StatValue BuffDuration;
        }

        private FlameInjectStats CalcStats()
        {
            int fireCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            return new FlameInjectStats
            {
                DotDps = new StatValue("附加每秒火焰伤害", Config.BaseDotDps, fireCellCount * Config.DotDpsPerFireCell),
                DotDuration = new StatValue("火焰伤害持续时间", Config.DotDuration),
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
                $"{stats.BuffDuration}";
        }
        public static string DescriptionStatic() => "攻击时附加火焰伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "炎附";

        public void ApplyPassive()
        {
            var stats = CalcStats();
            var buff = new Buffs.FlameInject(
                stats.DotDps.Final,
                stats.DotDuration.Final,
                stats.BuffDuration.Final,
                Owner,
                this
            );
            Owner.AddBuff(buff);
        }

    }
}