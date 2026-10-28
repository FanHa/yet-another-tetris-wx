using System.Linq;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    // 仿照 FlameInject：被动技能。命中时若目标有 Chilled Buff -> 计算层数额外伤害并移除该 Buff
    public class IceBreaker : Skill
    {
        private IceBreakerSkillDefinition SkillDefinition => (IceBreakerSkillDefinition)Definition;

        public override string DescribeEffect()
        {
            return Definition.Description + ":\n" + CreateBuff().DescribeAppliedEffect();
        }

        public override string Name() => Definition.DisplayName;

        public override void OnPassiveEffectsApplication()
        {
            OwnerContext.AddBuff(CreateBuff());
        }

        private Buffs.IceBreaker CreateBuff()
        {
            return new Buffs.IceBreaker(
                SkillDefinition.IceBreakerBuffDefinition,
                OwnerContext.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(OwnerContext.Owner, this)
            );
        }
    }
}