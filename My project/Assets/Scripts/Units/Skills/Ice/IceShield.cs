using Model.Resonance;
using Model.Buffs;
using Model.Skills;
using UnityEngine;
namespace Units.Skills
{
    public class IceShield : Skill
    {
        private IceShieldSkillDefinition SkillDefinition => (IceShieldSkillDefinition)Definition;

        public override string DescribeEffect()
        {
            return
                Definition.Description + ":\n" + CreateBuff().DescribeAppliedEffect();
        }

        public override string Name() => Definition.DisplayName;

        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(CreateBuff());
        }

        private Buffs.IceShield CreateBuff() => new(
            SkillDefinition.IceShieldBuffDefinition,
            Context.GetResonanceCount(Definition.Resonance),
            new Buffs.BuffSource(Context.Unit, this));
    }
}