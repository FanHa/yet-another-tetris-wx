using Model.Tetri;
using Model.Skills;

namespace Units.Skills
{
    public class LifeEcho : Skill
    {
        private LifeEchoSkillDefinition SkillDefinition => (LifeEchoSkillDefinition)Definition;

        public override string Name() => Definition.DisplayName;

        public override string DescribeEffect() => Definition.Description + ":\n" + CreateBuff().DescribeAppliedEffect();

        public override void OnPassiveEffectsApplication()
        {
            Owner.AddBuff(CreateBuff());
        }

        private Units.Buffs.LifeEchoBuff CreateBuff() => new(
                SkillDefinition.LifeEchoBuffDefinition,
                Owner.GetResonanceCount(Definition.Resonance),
                new Units.Buffs.BuffSource(Owner, this));
    }
}