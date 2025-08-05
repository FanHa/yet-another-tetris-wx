using Units.Skills;

namespace Units.Buffs
{
    public class WindShiftBuff : Buff
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
            $"风形态：攻击距离+{attackRangeBonus}";

        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            unit.Attributes.AttackRange += attackRangeBonus;
            unit.Attributes.IsRanged = true;
        }


        public override void OnRemove(Unit unit)
        {
            unit.Attributes.AttackRange -= attackRangeBonus;
        }
    }
}