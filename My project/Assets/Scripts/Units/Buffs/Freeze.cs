using Units.Skills;

namespace Units.Buffs
{
    /// <summary>
    /// Freeze：完全冻结目标，无法移动、攻击、释放技能，能量回复为0
    /// </summary>
    public class Freeze : Buff
    {
        public Freeze(
            float duration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
        }

        public override string Name() => "Freeze";
        public override string Description() => "完全冻结，无法行动，能量回复为0";

        protected override void OnApplyExtra(Unit unit)
        {
            // 攻速、移速、能量回复全部-100%
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -100);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, -100);
            unit.Attributes.EnergyPerTick.AddPercentageModifier(this, -100);

        }

        public override void OnRemove(Unit unit)
        {
            unit.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            unit.Attributes.MoveSpeed.RemovePercentageModifier(this);
            unit.Attributes.EnergyPerTick.RemovePercentageModifier(this);

        }
    }
}