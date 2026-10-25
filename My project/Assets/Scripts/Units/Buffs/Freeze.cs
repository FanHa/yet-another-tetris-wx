using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// Freeze：完全冻结目标，无法移动、攻击、释放技能，能量回复为0
    /// </summary>
    public class Freeze : Buff
    {
        private readonly FreezeBuffDefinition freezeDefinition;
        private readonly int resonantCellCount;

        public Freeze(FreezeBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, BuffDuration.ForSeconds(definition.CalculateStats(resonantCellCount).FinalValue), source.SourceUnit, source.SourceSkill)
        {
            freezeDefinition = definition;
            this.resonantCellCount = resonantCellCount;
        }

        public override string Name() => freezeDefinition.DisplayName;
        public override string DescribeAppliedEffect() => freezeDefinition.DescribeEffect(resonantCellCount);

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            // 攻速、移速、能量回复全部-100%
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -100);
            context.Attributes.MoveSpeed.AddPercentageModifier(this, -100);
            context.Attributes.ActionSpeed.AddPercentageModifier(this, -100);
            context.Attributes.EnergyPerSecond.AddPercentageModifier(this, -100);
        }

        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            context.Attributes.MoveSpeed.RemovePercentageModifier(this);
            context.Attributes.ActionSpeed.RemovePercentageModifier(this);
            context.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();

        }
    }
}