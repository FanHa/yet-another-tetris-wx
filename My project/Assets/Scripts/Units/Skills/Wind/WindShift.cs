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
            Owner.AddBuff(new Buffs.WindShiftBuff(
                skillDefinition.WindShiftBuffDefinition,
                Owner.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Owner, this)
            ));
        }

        private Buffs.WindShiftBuff CreateBuff() => new(
            skillDefinition.WindShiftBuffDefinition,
            Owner.GetResonanceCount(Definition.Resonance),
            new Buffs.BuffSource(Owner, this)
        );
        
    }
}