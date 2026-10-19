using System.Linq;
using Model.Skills;
using Units.Buffs;
using UnityEngine;

namespace Units.Skills
{
    public class FlameInject : Skill
    {
        public FlameInjectLevelConfig Config { get; }

        public FlameInject(FlameInjectLevelConfig config)
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
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new FlameInjectStats
            {
                DotDps = new StatValue("附加每秒火焰伤害", Config.BaseDotDps, resonantCellCount * Config.DotDpsPerResonantCell),
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

        public override void OnApplyPassive()
        {
            var stats = CalcStats();
            var config = (FlameInjectSkillConfig)Definition.Config;
            var buff = BuffFactory.Create(
                config.FlameInjectBuffDefinition,
                new FlameInjectBuffArgs(
                    config.BurnBuffDefinition,
                    stats.DotDps.Final,
                    stats.DotDuration.Final,
                    stats.BuffDuration.Final,
                    new BuffSource(Owner.SelfUnit, this))
            );
            Owner.AddBuff(buff);
        }

    }
}