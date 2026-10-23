using Model;
using Model.Buffs;
using Units.Skills;

namespace Units.Buffs
{
    public class EnergyCurseBuff : Buff
    {
        private readonly EnergyCurseBuffDefinition energyCurseDefinition;
        private readonly int resonantCellCount;
        public float EnergyRegenReductionPercent { get; }
        private readonly string stackId;

        public EnergyCurseBuff(EnergyCurseBuffDefinition definition, int resonantCellCount, BuffSource source, string stackId = "")
            : this(definition, resonantCellCount, source, stackId, definition.CalculateStats(resonantCellCount))
        {
        }

        private EnergyCurseBuff(EnergyCurseBuffDefinition definition, int resonantCellCount, BuffSource source, string stackId,
            (string Duration, StatBreakdownDisplay EnergyRegenReductionPercent) stats)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            energyCurseDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            EnergyRegenReductionPercent = stats.EnergyRegenReductionPercent.FinalValue;
            this.stackId = stackId;
        }

        public override string Name() => Definition.DisplayName;
        public override string GetKey() => $"{base.GetKey()}:{stackId}";
        public override string Description() => energyCurseDefinition.DescribeEffect(resonantCellCount);

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.EnergyPerSecond.AddPercentageModifier(this, -EnergyRegenReductionPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}
