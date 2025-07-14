using Units.Skills;

namespace Units.Buffs
{
    public class WindShiftBuff : Buff, IAttackHitTrigger, ITakeHitTrigger
    {
        private const float AttackRangeBonus = 2.0f;      // 攻击距离提升
        private const float DamageMultiplier = 0.5f;      // 造成伤害降低到50%
        private const float TakeDamageMultiplier = 2.0f;  // 受到伤害提升到200%

        public WindShiftBuff(float duration, Unit sourceUnit, Skill sourceSkill)
            : base(duration, sourceUnit, sourceSkill)
        {
        }

        public override string Name() => "风形态";
        public override string Description() =>
            "风形态：攻击距离+2，造成伤害-30%，受到伤害+30%";

        protected override void OnApplyExtra(Unit unit)
        {
            // 提升攻击距离
            unit.Attributes.AttackRange += AttackRangeBonus;
            unit.Attributes.IsRanged = true; // 设置为远程单位
        }

        public override void OnRemove(Unit unit)
        {
            // 恢复攻击距离
            unit.Attributes.AttackRange -= AttackRangeBonus;
        }

        // 造成伤害时降低输出
        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            damage.SetValue(damage.Value * DamageMultiplier);
        }

        // 受到伤害时提升所受伤害
        public void OnTakeHit(Unit self, Unit attacker, ref Damages.Damage damage)
        {
            damage.SetValue(damage.Value * TakeDamageMultiplier);
        }
    }
}