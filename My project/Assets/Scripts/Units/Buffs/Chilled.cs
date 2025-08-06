using Units.Skills;

namespace Units.Buffs
{
    /// <summary>
    /// Chilled：多重减速Debuff
    /// </summary>
    public class Chilled : Buff
    {
        public int MoveSlowPercent { get; }
        public int AttackSlowPercent { get; }
        public int EnergyRegenSlowPercent { get; }

        public Chilled(
            float duration,
            int moveSlowPercent,
            int attackSlowPercent,
            int energyRegenSlowPercent,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
            MoveSlowPercent = moveSlowPercent;
            AttackSlowPercent = attackSlowPercent;
            EnergyRegenSlowPercent = energyRegenSlowPercent;
        }

        public override string Name() => "Chilled";
        public override string Description() =>
            $"移速-{MoveSlowPercent}%，攻速-{AttackSlowPercent}%，能量回复-{EnergyRegenSlowPercent}%";


        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -AttackSlowPercent);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, -MoveSlowPercent);
            unit.Attributes.EnergyPerSecond.AddPercentageModifier(this, -EnergyRegenSlowPercent);
        }
        public override void OnRemove()
        {
            owner.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            owner.Attributes.MoveSpeed.RemovePercentageModifier(this);
            owner.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}