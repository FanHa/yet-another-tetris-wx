using Model.Tetri;
using UnityEngine;
using Wangsu.WcsLib.Core;

namespace Units.Skills
{
    public class IceShield : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.IceShield;

        public IceShieldConfig Config { get; }

        public IceShield(IceShieldConfig config)
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
                $"{stats.EnergySlowPercent}";
        }

        public static string DescriptionStatic() => "获得一次性冰霜护盾，被攻击时反制攻击者，降低其移速、攻速和能量获取速度.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰霜护盾";

        public void ApplyPassive()
        {
            var stats = CalcStats();
            var buff = new Buffs.IceShield(
                stats.BuffDuration.Final,
                stats.ChilledDuration.Final,
                (int)stats.MoveSlowPercent.Final,
                (int)stats.AtkSlowPercent.Final,
                (int)stats.EnergySlowPercent.Final,
                Owner,
                this
            );

            Owner.AddBuff(buff);
        }

        private struct IceShieldStats
        {
            public StatValue BuffDuration;
            public StatValue ChilledDuration;
            public StatValue MoveSlowPercent;
            public StatValue AtkSlowPercent;
            public StatValue EnergySlowPercent;
        }

        private IceShieldStats CalcStats()
        {
            int iceCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            return new IceShieldStats
            {
                BuffDuration = new StatValue("护盾持续时间", Config.BuffDuration),
                ChilledDuration = new StatValue("反制减速持续", Config.BaseChilledDuration, iceCellCount * Config.ChilledDurationAdditionPerIceCell),
                MoveSlowPercent = new StatValue("移动速度降低(%)", Config.BaseMoveSlowPercent, iceCellCount * Config.MoveSlowPercentPerIceCell),
                AtkSlowPercent = new StatValue("攻击速度降低(%)", Config.BaseAtkSlowPercent, iceCellCount * Config.AtkSlowPercentPerIceCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseEnergySlowPercent, iceCellCount * Config.EnergySlowPercentPerIceCell)
            };
        }
    }
}