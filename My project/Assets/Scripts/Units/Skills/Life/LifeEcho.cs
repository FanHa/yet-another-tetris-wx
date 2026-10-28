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
            Context.AddBuff(CreateBuff());
        }

        private Units.Buffs.LifeEchoBuff CreateBuff() => new(
                SkillDefinition.LifeEchoBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Units.Buffs.BuffSource(Context.Unit, this));
    }
}