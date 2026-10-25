using Model.Resonance;
using Model.Buffs;
using Model.Skills;
using UnityEngine;
namespace Units.Skills
{
    public class IceShield : Skill
    {
        public IceShieldLevelConfig Config { get; }

        public IceShield(IceShieldLevelConfig config)
        {
            Config = config;
        }

        public override string DescribeEffect()
        {
            return
                DescriptionStatic() + ":\n" +
                ((IceShieldSkillConfig)Definition.Config).IceShieldBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "获得一次性冰霜护盾，被攻击时反制攻击者，降低其移速、攻速和动作速率.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰霜护盾";

        public override void OnPassiveEffectsApplication()
        {
            var config = (IceShieldSkillConfig)Definition.Config;
            var buff = new Buffs.IceShield(
                config.IceShieldBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            );

            Context.AddBuff(buff);
        }

    }
}