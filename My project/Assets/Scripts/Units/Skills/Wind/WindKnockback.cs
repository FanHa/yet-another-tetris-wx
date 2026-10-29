using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class WindKnockback : Skill
    {
        private readonly WindKnockbackSkillDefinition skillDefinition;

        public WindKnockback(WindKnockbackSkillDefinition definition)
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
            Owner.AddBuff(new Buffs.WindKnockbackBuff(
                skillDefinition.WindKnockbackBuffDefinition,
                Owner.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Owner, this)
            ));
        }

        private Buffs.WindKnockbackBuff CreateBuff() => new(
            skillDefinition.WindKnockbackBuffDefinition,
            Owner.GetResonanceCount(Definition.Resonance),
            new Buffs.BuffSource(Owner, this)
        );
    }
}
