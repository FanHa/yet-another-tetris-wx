using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public class LifeShieldBuff : Buff, ITakeHitTrigger
    {
        private float shieldValue;           // 当前护盾值
        private float absorbPercent;         // 每次受到伤害时，吸收伤害的百分比（如 50 表示吸收50%）

        public LifeShieldBuff(float shieldValue, float absorbPercent, float duration, Unit sourceUnit, Skill sourceSkill)
            : base(duration, sourceUnit, sourceSkill)
        {
            this.shieldValue = shieldValue;
            this.absorbPercent = absorbPercent; // 例如 50 表示吸收50%
        }

        public override string Name() => "生命护盾";
        public override string Description() => $"受到伤害时吸收{absorbPercent}%伤害，最多吸收{shieldValue}点，持续{duration}秒";

        public void OnTakeHit(Unit self, Unit attacker, ref Damages.Damage damage)
        {
            if (shieldValue <= 0) return;

            float percent = absorbPercent / 100f;
            float absorbAmount = Mathf.Min(shieldValue, damage.Value * percent);

            damage.SetValue(damage.Value - absorbAmount);
            shieldValue -= absorbAmount;

            if (shieldValue <= 0)
            {
                self.RemoveBuff(this);
            }
        }
    }
}