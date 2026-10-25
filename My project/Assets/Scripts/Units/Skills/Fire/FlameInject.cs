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

        public override string DescribeEffect()
        {
            return
                DescriptionStatic() + ":\n" +
                ((FlameInjectSkillConfig)Definition.Config).FlameInjectBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
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
                Context.GetResonanceCount(Definition.Resonance),
                new BuffSource(Context.Unit, this)
            );
            Context.AddBuff(buff);
        }

    }
}