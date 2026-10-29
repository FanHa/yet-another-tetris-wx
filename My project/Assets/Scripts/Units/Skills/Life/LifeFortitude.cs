using Model.Skills;

namespace Units.Skills
{
    public class LifeFortitude : Skill
    {
        private LifeFortitudeSkillDefinition SkillDefinition => (LifeFortitudeSkillDefinition)Definition;

        public override string Name() => Definition.DisplayName;

        public override string DescribeEffect() => Definition.Description + "\n" + CreateBuff().DescribeAppliedEffect();

        public override void OnPassiveEffectsApplication()
        {
            Owner.AddBuff(CreateBuff());
        }

        private Buffs.LifeFortitudeBuff CreateBuff() => new(
                SkillDefinition.BuffDefinition,
                SkillDefinition.MaxHealthIncrease,
                Owner.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Owner, this));
    }
}