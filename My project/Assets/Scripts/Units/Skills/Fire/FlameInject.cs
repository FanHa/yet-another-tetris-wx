using Model.Skills;
using Units.Buffs;

namespace Units.Skills
{
    public class FlameInject : Skill
    {
        private FlameInjectSkillDefinition SkillDefinition => (FlameInjectSkillDefinition)Definition;

        public override string DescribeEffect()
        {
            var previewBuff = CreateBuff();
            return $"{Definition.Description}\n{previewBuff.DescribeEffect()}";
        }

        public override string Name() => Definition.DisplayName;

        public override void OnPassiveEffectsApplication()
        {
            Owner.AddBuff(CreateBuff());
        }

        private Units.Buffs.FlameInject CreateBuff()
        {
            return new Units.Buffs.FlameInject(
                SkillDefinition.FlameInjectBuffDefinition,
                Owner.GetResonanceCount(Definition.Resonance),
                new BuffSource(Owner, this)
            );
        }

    }
}