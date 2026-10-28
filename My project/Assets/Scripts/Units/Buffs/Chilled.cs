using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    /// <summary>
    /// Chilled：多重减速Debuff
    /// </summary>
    public class Chilled : Buff
    {
        private readonly ChilledBuffDefinition chilledDefinition;
        public int MoveSlowPercent { get; }
        public int AttackSlowPercent { get; }
        public int ActionSlowPercent { get; }

        public Chilled(ChilledBuffDefinition definition, BuffDuration duration, int moveSlowPercent, int attackSlowPercent, int actionSlowPercent, BuffSource source)
            : base(definition, duration, source.SourceUnit, source.SourceSkill)
        {
            chilledDefinition = definition;
            MoveSlowPercent = moveSlowPercent;
            AttackSlowPercent = attackSlowPercent;
            ActionSlowPercent = actionSlowPercent;
        }

        public override string Name() => chilledDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"{Definition.Description}\n" +
            Duration.ToDisplayString("减速持续时间") + "\n" +
            StatBreakdownFormatter.Format(StatLabel.MovementSpeedReductionPercent, new AdditiveValueBreakdown(MoveSlowPercent)) + "\n" +
            StatBreakdownFormatter.Format(StatLabel.AttackSpeedReductionPercent, new AdditiveValueBreakdown(AttackSlowPercent)) + "\n" +
            StatBreakdownFormatter.Format(StatLabel.ActionRateReductionPercent, new AdditiveValueBreakdown(ActionSlowPercent));


        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -AttackSlowPercent);
            context.Attributes.MoveSpeed.AddPercentageModifier(this, -MoveSlowPercent);
            context.Attributes.ActionSpeed.AddPercentageModifier(this, -ActionSlowPercent);
        }
        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            context.Attributes.MoveSpeed.RemovePercentageModifier(this);
            context.Attributes.ActionSpeed.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}