using Model.Buffs;

namespace Units.Buffs
{
    public sealed class LifeFortitudeBuff : Buff
    {
        private readonly LifeFortitudeBuffDefinition buffDefinition;
        private readonly float baseMaxHealthIncrease;
        private readonly int resonantCellCount;
        private readonly float maxHealthIncrease;

        public LifeFortitudeBuff(
            LifeFortitudeBuffDefinition definition,
            float baseMaxHealthIncrease,
            int resonantCellCount,
            BuffSource source
        ) : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            buffDefinition = definition;
            this.baseMaxHealthIncrease = baseMaxHealthIncrease;
            this.resonantCellCount = resonantCellCount;
            maxHealthIncrease = definition.CalculateMaxHealthIncrease(baseMaxHealthIncrease, resonantCellCount);
        }

        public override string Name() => buffDefinition.DisplayName;

        public override string Description() =>
            buffDefinition.DescribeEffect(baseMaxHealthIncrease, resonantCellCount);

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