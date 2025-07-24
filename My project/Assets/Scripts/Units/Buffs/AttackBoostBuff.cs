using Units.Skills;

namespace Units.Buffs
{
    /// <summary>
    /// 疾风加速Buff：短时间提升攻击速度
    /// </summary>
    public class AttackBoostBuff : Buff
    {
        public float AtkSpeedPercent { get; }

        public AttackBoostBuff(
            float duration,
            float atkSpeedPercent,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
            AtkSpeedPercent = atkSpeedPercent;
        }

        public override string Name() => "疾风加速";
        public override string Description() => $"攻击速度提升{AtkSpeedPercent}%";

        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, AtkSpeedPercent);
        }

        public override void OnRemove(Unit unit)
        {
            unit.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
        }
    }
}