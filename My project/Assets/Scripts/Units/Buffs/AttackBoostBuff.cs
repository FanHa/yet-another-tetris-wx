using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    /// <summary>
    /// 疾风加速Buff：短时间提升攻击速度
    /// </summary>
    public class AttackBoostBuff : Buff
    {
        private readonly AttackBoostBuffDefinition attackBoostDefinition;
        private readonly AdditiveValueBreakdown atkSpeedPercentStats;
        public float AtkSpeedPercent { get; }

        public AttackBoostBuff(AttackBoostBuffDefinition definition, int resonantCellCount, BuffSource source)
            : this(definition, source, definition.CalculateStats(resonantCellCount))
        {
        }

        private AttackBoostBuff(AttackBoostBuffDefinition definition, BuffSource source,
            (BuffDuration Duration, AdditiveValueBreakdown AtkSpeedPercent) stats)
            : base(definition, stats.Duration, source.SourceUnit, source.SourceSkill)
        {
            attackBoostDefinition = definition;
            atkSpeedPercentStats = stats.AtkSpeedPercent;
            AtkSpeedPercent = stats.AtkSpeedPercent.FinalValue;
        }

        public override string Name() => attackBoostDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"{Duration.ToDisplayString("持续时间")}\n" +
            StatBreakdownFormatter.Format(StatLabel.AttackSpeedIncreasePercent, atkSpeedPercentStats);

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, AtkSpeedPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}