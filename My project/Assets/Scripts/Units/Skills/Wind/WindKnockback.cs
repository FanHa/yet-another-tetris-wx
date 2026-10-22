using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class WindKnockback : Skill
    {
        public WindKnockbackLevelConfig Config { get; }

        public WindKnockback(WindKnockbackLevelConfig config)
        {
            Config = config;
        }

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                ((WindKnockbackSkillConfig)Definition.Config).WindKnockbackBuffDefinition.DescribeEffect(GetResonantCellCount());
        }

        public static string DescriptionStatic() => "普通攻击命中时对目标造成小幅击退";

        public override string Name() => NameStatic();

        public static string NameStatic() => "轻风击退";

        public override void OnPassiveEffectsApplication()
        {
            Owner.AddBuff(new Buffs.WindKnockbackBuff(
                ((WindKnockbackSkillConfig)Definition.Config).WindKnockbackBuffDefinition,
                GetResonantCellCount(),
                new Buffs.BuffSource(Owner.SelfUnit, this)
            ));
        }
    }
}
