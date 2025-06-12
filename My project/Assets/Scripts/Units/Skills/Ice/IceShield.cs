using UnityEngine;

namespace Units.Skills
{
    public class IceShield : Skill
    {
        private bool hasTriggered;

        public int BaseMoveSlowPercent = 10;
        public int MoveSlowPercentPerIceCell = 5;
        public int BaseAtkSlowPercent = 10;
        public int AtkSlowPercentPerIceCell = 5;
        public int BaseEnergySlowPercent = 15; 
        public int EnergySlowPercentPerIceCell = 5;

        public float BuffDuration = -1f;
        public float BaseChilledDuration = 5f;
        public float chilledDurationAdditionPerIceCell = 0.5f;
        private int iceCellCount = 0;

        public IceShield()
        {
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
            float buffDuration = BuffDuration;
            float chilledDuration = BaseChilledDuration + iceCellCount * chilledDurationAdditionPerIceCell;
            int moveSlowPercent = BaseMoveSlowPercent + iceCellCount * MoveSlowPercentPerIceCell;
            int atkSlowPercent = BaseAtkSlowPercent + iceCellCount * AtkSlowPercentPerIceCell;
            int energySlowPercent = BaseEnergySlowPercent + iceCellCount * EnergySlowPercentPerIceCell;

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