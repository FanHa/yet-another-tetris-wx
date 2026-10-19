using Model.Resonance;
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
            return
                DescriptionStatic() + ":\n" +
                $"{stats.BuffDuration}\n" +
                $"{stats.ChilledDuration}\n" +
                $"{stats.MoveSlowPercent}\n" +
                $"{stats.AtkSlowPercent}\n" +
                $"{stats.ActionSlowPercent}\n" +
                $"{stats.EnergySlowPercent}";
        }

        public static string DescriptionStatic() => "获得一次性冰霜护盾，被攻击时反制攻击者，降低其移速、攻速和能量获取速度.";

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
                    stats.ChilledDuration.Final,
                    (int)stats.MoveSlowPercent.Final,
                    (int)stats.AtkSlowPercent.Final,
                    (int)stats.ActionSlowPercent.Final,
                    (int)stats.EnergySlowPercent.Final,
                    new Buffs.BuffSource(Owner.SelfUnit, this))
            );

            Owner.AddBuff(buff);
        }

        private struct IceShieldStats
        {
            public StatValue BuffDuration;
            public StatValue ChilledDuration;
            public StatValue MoveSlowPercent;
            public StatValue AtkSlowPercent;
            public StatValue ActionSlowPercent;
            public StatValue EnergySlowPercent;
        }

        private IceShieldStats CalcStats()
        {
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new IceShieldStats
            {
                BuffDuration = new StatValue("护盾持续时间", Config.BuffDuration),
                ChilledDuration = new StatValue("反制减速持续", Config.BaseChilledDuration, resonantCellCount * Config.ChilledDurationAdditionPerResonantCell),
                MoveSlowPercent = new StatValue("移动速度降低(%)", Config.BaseMoveSlowPercent, resonantCellCount * Config.MoveSlowPercentPerResonantCell),
                AtkSlowPercent = new StatValue("攻击速度降低(%)", Config.BaseAtkSlowPercent, resonantCellCount * Config.AtkSlowPercentPerResonantCell),
                ActionSlowPercent = new StatValue("动作速率降低(%)", Config.BaseActionSlowPercent, resonantCellCount * Config.ActionSlowPercentPerResonantCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseEnergySlowPercent, resonantCellCount * Config.EnergySlowPercentPerResonantCell)
            };
        }
    }
}