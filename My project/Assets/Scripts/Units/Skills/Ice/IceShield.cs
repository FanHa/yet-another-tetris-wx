using UnityEngine;
using Wangsu.WcsLib.Core;

namespace Units.Skills
{
    public class IceShield : Skill
    {
        private bool hasTriggered;
        private int iceCellCount = 0;
        public IceShieldConfig Config { get; }

        public IceShield(IceShieldConfig config)
        {
            Config = config;
            hasTriggered = false;
        }

        public void SetIceCellCount(int iceCellCount)
        {
            this.iceCellCount = iceCellCount;
        }

        public override bool IsReady()
        {
            return !hasTriggered;
        }

        protected override void ExecuteCore(Unit caster)
        {
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
            TriggerEffect(new SkillEffectContext
            {
                Skill = this,
                Target = caster
            });
            hasTriggered = true;
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