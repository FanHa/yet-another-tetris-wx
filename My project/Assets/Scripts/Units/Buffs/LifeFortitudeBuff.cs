using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    public sealed class LifeFortitudeBuff : Buff
    {
        private readonly LifeFortitudeBuffDefinition buffDefinition;
        private readonly AdditiveValueBreakdown maxHealthIncreaseBreakdown;
        private readonly float maxHealthIncrease;

        public LifeFortitudeBuff(
            LifeFortitudeBuffDefinition definition,
            float baseMaxHealthIncrease,
            int resonantCellCount,
            BuffSource source
        ) : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            buffDefinition = definition;
            maxHealthIncreaseBreakdown = definition.CalculateStats(baseMaxHealthIncrease, resonantCellCount);
            maxHealthIncrease = maxHealthIncreaseBreakdown.FinalValue;
        }

        public override string Name() => buffDefinition.DisplayName;

        public override string DescribeAppliedEffect() =>
            $"{Definition.Description}\n" +
            StatBreakdownFormatter.Format(StatLabel.MaximumHealth, maxHealthIncreaseBreakdown) + "\n" +
            Duration.ToDisplayString("效果持续时间");

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.MaxHealth.AddFlatModifier(this, maxHealthIncrease);
        }

        public override void OnRemove()
        {
            context.Attributes.MaxHealth.RemoveFlatModifier(this);
            base.OnRemove();
        }
    }
}