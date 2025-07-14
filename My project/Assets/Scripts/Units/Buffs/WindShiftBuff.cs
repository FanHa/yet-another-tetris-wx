using Units.Skills;

namespace Units.Buffs
{
    public class WindShiftBuff : Buff, IAttackHitTrigger, ITakeHitTrigger
    {
        private readonly float attackRangeBonus;      // 攻击距离提升
        private readonly float damageReducePercent;   // 造成伤害降低百分比（如30表示降低30%）
        private readonly float takeDamageIncreasePercent; // 受到伤害提升百分比（如30表示提升30%）


        public WindShiftBuff(
            float duration,
            Unit sourceUnit,
            Skill sourceSkill,
            float attackRangeBonus,
            float damageReducePercent,
            float takeDamageIncreasePercent
        )
            : base(duration, sourceUnit, sourceSkill)
        {
            this.attackRangeBonus = attackRangeBonus;
            this.damageReducePercent = damageReducePercent;
            this.takeDamageIncreasePercent = takeDamageIncreasePercent;
        }

        public override string Name() => "风形态";
        public override string Description() =>
            $"风形态：攻击距离+{attackRangeBonus}，造成伤害-{damageReducePercent}% ，受到伤害+{takeDamageIncreasePercent}%";

        protected override void OnApplyExtra(Unit unit)
        {
            unit.Attributes.AttackRange += attackRangeBonus;
            unit.Attributes.IsRanged = true;
        }


        public override void OnRemove(Unit unit)
        {
            unit.Attributes.AttackRange -= attackRangeBonus;
        }

        // 造成伤害时降低输出
        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            float multiplier = 1f - damageReducePercent / 100f;
            damage.SetValue(damage.Value * multiplier);
        }


        // 受到伤害时提升所受伤害
        public void OnTakeHit(Unit self, Unit attacker, ref Damages.Damage damage)
        {
            float multiplier = 1f + takeDamageIncreasePercent / 100f;
            damage.SetValue(damage.Value * multiplier);
        }
    }
}