using Model.Tetri;
using UnityEngine;
using Wangsu.WcsLib.Core;

namespace Units.Skills
{
    public class IceShield : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.IceShield;

        private bool hasTriggered;
        public IceShieldConfig Config { get; }

        public IceShield(IceShieldConfig config)
        {
            Config = config;
            hasTriggered = false;
        }

        public override bool IsReady()
        {
            return !hasTriggered;
        }

        protected override void ExecuteCore(Unit caster)
        {
            hasTriggered = true;
            int iceCellCount = caster.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
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
                caster,
                this
            );
            caster.AddBuff(buff);
        }

        public override string Description()
        {
            return $"获得一次性冰霜护盾，被攻击时反制攻击者，降低其移速、攻速和能量获取速度，所有效果随冰系Cell数量提升。";
        }

        public override string Name()
        {
            return "冰霜护盾";
        }
    }
}