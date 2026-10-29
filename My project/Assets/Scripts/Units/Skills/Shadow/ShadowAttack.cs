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
            Owner.AddBuff(CreateBuff());
        }

        private Buffs.ShadowAttackBuff CreateBuff()
        {
            return new Buffs.ShadowAttackBuff(
                skillDefinition.ShadowAttackBuffDefinition,
                Owner.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Owner, this)
            );
        }
    }
}