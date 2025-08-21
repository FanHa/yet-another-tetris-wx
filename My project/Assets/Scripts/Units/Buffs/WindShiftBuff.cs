using Units.Skills;

namespace Units.Buffs
{
    public class WindShiftBuff : Buff, IAttackHitTrigger
    {
        private readonly float attackRangeBonus;      // 攻击距离提升）

        public WindShiftBuff(
            float duration,
            Unit sourceUnit,
            Skill sourceSkill,
            float attackRangeBonus
        )
            : base(duration, sourceUnit, sourceSkill)
        {
            this.attackRangeBonus = attackRangeBonus;
        }

        public override string Name() => "风形态";
        public override string Description() =>
            $"攻击距离+{attackRangeBonus}，攻击时获得额外的与攻击距离相关的伤害加成";


        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            unit.Attributes.AttackRange.AddFlatModifier(this, attackRangeBonus);
        }


        public override void OnRemove()
        {
            owner.Attributes.AttackRange.RemoveFlatModifier(this);
            base.OnRemove();
        }

        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            // 附加伤害与当前攻击距离相关（可配合其他攻击距离加成机制）
            float extraDamage = attacker.Attributes.AttackRange.finalValue;
            var windDamage = new Damages.Damage(extraDamage, Damages.DamageType.Extra);
            windDamage.SetSourceUnit(attacker);
            windDamage.SetTargetUnit(target);
            windDamage.SetSourceLabel("风形态加成");

            target.TakeDamage(windDamage);
        }
    }
}