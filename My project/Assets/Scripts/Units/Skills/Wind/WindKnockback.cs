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

        public override string DescribeEffect()
        {
            return
                DescriptionStatic() + ":\n" +
                ((WindKnockbackSkillConfig)Definition.Config).WindKnockbackBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "普通攻击命中时对目标造成小幅击退";

        public override string Name() => NameStatic();

        public static string NameStatic() => "轻风击退";

        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(new Buffs.WindKnockbackBuff(
                ((WindKnockbackSkillConfig)Definition.Config).WindKnockbackBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            ));
        }
    }
}
