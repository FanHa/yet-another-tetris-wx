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

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                ((IceShieldSkillConfig)Definition.Config).IceShieldBuffDefinition.DescribeEffect(GetResonantCellCount());
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
                GetResonantCellCount(),
                new Buffs.BuffSource(Owner.SelfUnit, this)
            );

            Owner.AddBuff(buff);
        }

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }
    }
}