using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public class LifeShieldBuff : Buff
    {
        private float shieldValue;           // 当前护盾值
        private float absorbPercent;         // 每次受到伤害时，吸收伤害的百分比（如 50 表示吸收50%）
        private Shield shield;               // 护盾对象
        public LifeShieldBuff(float shieldValue, float duration, Unit sourceUnit, Skill sourceSkill)
            : base(duration, sourceUnit, sourceSkill)
        {
            this.shieldValue = shieldValue;
        }

        public override string Name() => "生命护盾";
        public override string Description() => $"受到伤害时吸收{absorbPercent}%伤害，最多吸收{shieldValue}点，持续{duration}秒";

        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            shield = new Shield(this, shieldValue);
            shield.OnBroken += OnShieldBroken;
            unit.Attributes.AddShield(shield);
        }

        public override void OnRemove(Unit unit)
        {
            shield.OnBroken -= OnShieldBroken;
            unit.Attributes.RemoveShield(shield);
        }

        private void OnShieldBroken(Shield shield)
        {
            // 护盾破碎时的处理逻辑
            // 例如：播放特效、移除Buff等
            owner.RemoveBuff(this);
        }
    }
}