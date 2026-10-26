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
            var previewBuff = CreateBuff();
            return $"{Definition.Description}\n{previewBuff.DescribeEffect()}";
        }

        public override string Name() => Definition.DisplayName;

        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(CreateBuff());
        }

        private Units.Buffs.FlameInject CreateBuff()
        {
            var config = (FlameInjectSkillConfig)Definition.Config;
            return new Units.Buffs.FlameInject(
                config.FlameInjectBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new BuffSource(Context.Unit, this)
            );
        }

    }
}