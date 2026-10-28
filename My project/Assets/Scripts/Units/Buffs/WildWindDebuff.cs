using Model;
using Units.Skills;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    /// <summary>
    /// 狂风减益：降低移动速度和攻击力
    /// </summary>
    public class WildWindDebuff : Buff
    {
        private readonly WildWindDebuffDefinition wildWindDefinition;
        private readonly AdditiveValueBreakdown moveSlowPercentStats;
        private readonly AdditiveValueBreakdown atkReducePercentStats;
        public int MoveSlowPercent { get; }
        public int AtkReducePercent { get; }

        public WildWindDebuff(WildWindDebuffDefinition definition, int resonantCellCount, BuffSource source)
            : this(definition, resonantCellCount, source, definition.CalculateStats(resonantCellCount))
        {
        }

        private WildWindDebuff(WildWindDebuffDefinition definition, int resonantCellCount, BuffSource source,
            (BuffDuration Duration, AdditiveValueBreakdown MoveSlowPercent, AdditiveValueBreakdown AtkReducePercent) stats)
            : base(definition, stats.Duration, source.SourceUnit, source.SourceSkill)
        {
            wildWindDefinition = definition;
            moveSlowPercentStats = stats.MoveSlowPercent;
            atkReducePercentStats = stats.AtkReducePercent;
            MoveSlowPercent = (int)stats.MoveSlowPercent.FinalValue;
            AtkReducePercent = (int)stats.AtkReducePercent.FinalValue;
        }

        public override string Name() => wildWindDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"{Duration.ToDisplayString("持续时间")}\n" +
            $"{StatBreakdownFormatter.Format(StatLabel.WildWindMovementSpeedReductionPercent, moveSlowPercentStats)}\n" +
            StatBreakdownFormatter.Format(StatLabel.WildWindAttackReductionPercent, atkReducePercentStats);

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.MoveSpeed.AddPercentageModifier(this, -MoveSlowPercent);
            context.Attributes.AttackPower.AddPercentageModifier(this, -AtkReducePercent);
        }

        public override void OnRemove()
        {
            context.Attributes.MoveSpeed.RemovePercentageModifier(this);
            context.Attributes.AttackPower.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}