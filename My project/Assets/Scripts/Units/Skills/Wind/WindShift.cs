using Model.Resonance;
using Model.Skills;

namespace Units.Skills
{
    public class WindShift : Skill
    {
        private readonly WindShiftSkillDefinition skillDefinition;

        public WindShift(WindShiftSkillDefinition definition)
        {
            skillDefinition = definition;
        }

        public override string DescribeEffect()
        {
            return
                Definition.Description + ":\n" +
                CreateBuff().DescribeAppliedEffect();
        }

        public override string Name() => Definition.DisplayName;
    
        public override void OnPassiveEffectsApplication()
        {
            OwnerContext.AddBuff(new Buffs.WindShiftBuff(
                skillDefinition.WindShiftBuffDefinition,
                OwnerContext.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(OwnerContext.Owner, this)
            ));
        }

        private Buffs.WindShiftBuff CreateBuff() => new(
            skillDefinition.WindShiftBuffDefinition,
            OwnerContext.GetResonanceCount(Definition.Resonance),
            new Buffs.BuffSource(OwnerContext.Owner, this)
        );
        
    }
}