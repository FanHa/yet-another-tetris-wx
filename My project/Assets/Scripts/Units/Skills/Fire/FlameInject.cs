using Model.Skills;
using Units.Buffs;

namespace Units.Skills
{
    public class FlameInject : Skill
    {
        public FlameInjectLevelConfig Config { get; }

        public FlameInject(FlameInjectLevelConfig config)
        {
            Config = config;
        }

        private int GetResonantCellCount()
        {
            return Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;
        }


        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                ((FlameInjectSkillConfig)Definition.Config).FlameInjectBuffDefinition.DescribeEffect(GetResonantCellCount());
        }
        public static string DescriptionStatic() => "攻击时附加火焰伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "炎附";

        public override void OnPassiveEffectsApplication()
        {
            var config = (FlameInjectSkillConfig)Definition.Config;
            var buff = new Units.Buffs.FlameInject(
                config.FlameInjectBuffDefinition,
                GetResonantCellCount(),
                new BuffSource(Context.Unit, this)
            );
            Context.AddBuff(buff);
        }

    }
}