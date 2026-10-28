using Model.Skills;

namespace Units.Skills
{
    // 仿照 FlameRing 的结构，具体公式与行为先占位
    public class EnergyAbsorb : Skill
    {
        private EnergyAbsorbSkillDefinition SkillDefinition => (EnergyAbsorbSkillDefinition)Definition;

        public override string DescribeEffect()
        {
            return CreateBuff().DescribeAppliedEffect();
        }

        public override string Name() => Definition.DisplayName;

        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(CreateBuff());
        }

        private Buffs.EnergyAbsorb CreateBuff()
        {
            return new Buffs.EnergyAbsorb(
                SkillDefinition.EnergyAbsorbBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            );
        }
    }
}