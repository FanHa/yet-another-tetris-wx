using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// 疾风加速Buff：短时间提升攻击速度
    /// </summary>
    public class AttackBoostBuff : Buff
    {
        private readonly AttackBoostBuffDefinition attackBoostDefinition;
        private readonly int resonantCellCount;
        public float AtkSpeedPercent { get; }

        public AttackBoostBuff(AttackBoostBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            attackBoostDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            AtkSpeedPercent = definition.CalculateStats(resonantCellCount).AtkSpeedPercent.FinalValue;
        }

        public override string Name() => attackBoostDefinition.DisplayName;
        public override string DescribeAppliedEffect() => attackBoostDefinition.DescribeEffect(resonantCellCount);

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