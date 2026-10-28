using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    /// <summary>
    /// 冰霜反击护盾：被攻击时对攻击者施加Chilled（多重减速）Debuff
    /// </summary>
    public class IceShield : Buff, ITakeHitTrigger
    {
        private readonly IceShieldBuffDefinition iceShieldDefinition;
        private readonly ChilledBuffDefinition chilledDefinition;
        private readonly AdditiveValueBreakdown chilledDuration;
        private readonly AdditiveValueBreakdown chilledMoveSlowPercent;
        private readonly AdditiveValueBreakdown chilledAttackSlowPercent;
        private readonly AdditiveValueBreakdown chilledActionSlowPercent;

        public IceShield(IceShieldBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            iceShieldDefinition = definition;
            chilledDefinition = definition.ChilledDefinition;
            var stats = chilledDefinition.CalculateStats(resonantCellCount);
            chilledDuration = stats.Duration;
            chilledMoveSlowPercent = stats.MoveSlowPercent;
            chilledAttackSlowPercent = stats.AttackSlowPercent;
            chilledActionSlowPercent = stats.ActionSlowPercent;
        }

        public override string Name() => iceShieldDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"{Definition.Description}\n" +
            Duration.ToDisplayString("护盾持续时间") + "\n" +
            $"{chilledDefinition.Description}\n" +
            StatBreakdownFormatter.Format(StatLabel.ChillDuration, chilledDuration) + "\n" +
            StatBreakdownFormatter.Format(StatLabel.MovementSpeedReductionPercent, chilledMoveSlowPercent) + "\n" +
            StatBreakdownFormatter.Format(StatLabel.AttackSpeedReductionPercent, chilledAttackSlowPercent) + "\n" +
            StatBreakdownFormatter.Format(StatLabel.ActionRateReductionPercent, chilledActionSlowPercent);

        public void OnTakeHit(IBuffContext context, Unit self, Unit attacker, ref Damages.Damage damage)
        {
            if (attacker == null)
                return;
            var chilled = new Chilled(
                chilledDefinition,
                BuffDuration.ForSeconds(chilledDuration.FinalValue),
                (int)chilledMoveSlowPercent.FinalValue,
                (int)chilledAttackSlowPercent.FinalValue,
                (int)chilledActionSlowPercent.FinalValue,
                new BuffSource(self, sourceSkill));
            context.AddBuffTo(attacker, chilled);
        }

    }
}