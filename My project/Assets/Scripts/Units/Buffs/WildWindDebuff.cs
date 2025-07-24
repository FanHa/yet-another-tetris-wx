using Units.Skills;

namespace Units.Buffs
{
    /// <summary>
    /// 狂风减益：降低移动速度和攻击力
    /// </summary>
    public class WildWindDebuff : Buff
    {
        public int MoveSlowPercent { get; }
        public int AtkReducePercent { get; }

        public WildWindDebuff(
            float duration,
            int moveSlowPercent,
            int atkReducePercent,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
            MoveSlowPercent = moveSlowPercent;
            AtkReducePercent = atkReducePercent;
        }

        public override string Name() => "狂风减益";
        public override string Description() =>
            $"移速-{MoveSlowPercent}%，攻击力-{AtkReducePercent}%";

        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, -MoveSlowPercent);
            unit.Attributes.AttackPower.AddPercentageModifier(this, -AtkReducePercent);
        }

        public override void OnRemove(Unit unit)
        {
            unit.Attributes.MoveSpeed.RemovePercentageModifier(this);
            unit.Attributes.AttackPower.RemovePercentageModifier(this);
        }
    }
}