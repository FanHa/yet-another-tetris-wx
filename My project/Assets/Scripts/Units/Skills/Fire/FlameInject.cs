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
            OwnerContext.AddBuff(CreateBuff());
        }

        private Units.Buffs.FlameInject CreateBuff()
        {
            return new Units.Buffs.FlameInject(
                SkillDefinition.FlameInjectBuffDefinition,
                OwnerContext.GetResonanceCount(Definition.Resonance),
                new BuffSource(OwnerContext.Owner, this)
            );
        }

    }
}