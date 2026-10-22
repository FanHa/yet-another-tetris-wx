using Model.Buffs;
using Units.Skills;

namespace Units.Buffs
{
    public class PowerCurseBuff : Buff
    {
        private readonly PowerCurseBuffDefinition powerCurseDefinition;
        private readonly int resonantCellCount;
        public float AttackReductionPercent { get; }
        private readonly string stackId;

        public PowerCurseBuff(PowerCurseBuffDefinition definition, int resonantCellCount, BuffSource source, string stackId = "")
            : this(definition, resonantCellCount, source, stackId, definition.CalculateStats(resonantCellCount))
        {
        }

        private PowerCurseBuff(PowerCurseBuffDefinition definition, int resonantCellCount, BuffSource source, string stackId,
            (string Duration, StatValue AttackReductionPercent) stats)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            powerCurseDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            AttackReductionPercent = stats.AttackReductionPercent.Final;
            this.stackId = stackId;
        }

        public override string Name() => Definition.DisplayName;
        public override string GetKey() => $"{base.GetKey()}:{stackId}";
        public override string Description() => powerCurseDefinition.DescribeEffect(resonantCellCount);

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttackPower.AddPercentageModifier(this, -AttackReductionPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.AttackPower.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}