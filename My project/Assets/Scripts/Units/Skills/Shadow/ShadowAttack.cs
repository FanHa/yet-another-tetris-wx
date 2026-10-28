using Model.Skills;

namespace Units.Skills
{
    public class ShadowAttack : Skill
    {
        private readonly ShadowAttackSkillDefinition skillDefinition;

        public ShadowAttack(ShadowAttackSkillDefinition definition)
        {
            skillDefinition = definition;
        }

        public override string DescribeEffect()
        {
            var buff = CreateBuff();
            return
                Definition.Description + ":\n" +
                buff.DescribeAppliedEffect() + "\n";
        }

        public override string Name() => Definition.DisplayName;

        public override void OnPassiveEffectsApplication()
        {
            OwnerContext.AddBuff(CreateBuff());
        }

        private Buffs.ShadowAttackBuff CreateBuff()
        {
            return new Buffs.ShadowAttackBuff(
                skillDefinition.ShadowAttackBuffDefinition,
                OwnerContext.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(OwnerContext.Owner, this)
            );
        }
    }
}