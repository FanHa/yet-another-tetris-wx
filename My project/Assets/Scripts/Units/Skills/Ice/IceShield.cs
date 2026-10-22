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
            var stats = CalcStats();
            var chilledDefinition = ((IceShieldSkillConfig)Definition.Config).ChilledBuffDefinition;
            return
                DescriptionStatic() + ":\n" +
                $"{stats.BuffDuration}\n" +
                chilledDefinition.DescribeEffect(GetResonantCellCount());
        }

        public static string DescriptionStatic() => "获得一次性冰霜护盾，被攻击时反制攻击者，降低其移速、攻速和动作速率.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰霜护盾";

        public override void OnApplyPassive()
        {
            var stats = CalcStats();
            var config = (IceShieldSkillConfig)Definition.Config;
            var buff = Buffs.BuffFactory.Create(
                config.IceShieldBuffDefinition,
                new Buffs.IceShieldBuffArgs(
                    config.ChilledBuffDefinition,
                    stats.BuffDuration.Final,
                    GetResonantCellCount(),
                    new Buffs.BuffSource(Owner.SelfUnit, this))
            );

            Owner.AddBuff(buff);
        }

        private struct IceShieldStats
        {
            public StatValue BuffDuration;
        }

        private IceShieldStats CalcStats()
        {
            return new IceShieldStats
            {
                BuffDuration = new StatValue("护盾持续时间", Config.BuffDuration)
            };
        }

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }
    }
}