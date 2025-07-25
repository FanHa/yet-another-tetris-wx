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
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "获得一次性冰霜护盾，被攻击时反制攻击者，降低其移速、攻速和能量获取速度.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰霜护盾";

        public void ApplyPassive()
        {
            int iceCellCount = Owner.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            float buffDuration = Config.BuffDuration;
            float chilledDuration = Config.BaseChilledDuration + iceCellCount * Config.ChilledDurationAdditionPerIceCell;
            int moveSlowPercent = Config.BaseMoveSlowPercent + iceCellCount * Config.MoveSlowPercentPerIceCell;
            int atkSlowPercent = Config.BaseAtkSlowPercent + iceCellCount * Config.AtkSlowPercentPerIceCell;
            int energySlowPercent = Config.BaseEnergySlowPercent + iceCellCount * Config.EnergySlowPercentPerIceCell;

            var buff = new Buffs.IceShield(
                buffDuration,
                chilledDuration,
                moveSlowPercent,
                atkSlowPercent,
                energySlowPercent,
                Owner,
                this
            );

            Owner.AddBuff(buff);
        }
    }
}