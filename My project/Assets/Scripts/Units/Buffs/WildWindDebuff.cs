using Model;
using Units.Skills;
using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// 狂风减益：降低移动速度和攻击力
    /// </summary>
    public class WildWindDebuff : Buff
    {
        private readonly WildWindDebuffDefinition wildWindDefinition;
        private readonly int resonantCellCount;
        public int MoveSlowPercent { get; }
        public int AtkReducePercent { get; }

        public WildWindDebuff(WildWindDebuffDefinition definition, int resonantCellCount, BuffSource source)
            : this(definition, resonantCellCount, source, definition.CalculateStats(resonantCellCount))
        {
        }

        private WildWindDebuff(WildWindDebuffDefinition definition, int resonantCellCount, BuffSource source,
            (AdditiveValueBreakdown Duration, AdditiveValueBreakdown MoveSlowPercent, AdditiveValueBreakdown AtkReducePercent) stats)
            : base(definition, BuffDuration.ForSeconds(stats.Duration.FinalValue), source.SourceUnit, source.SourceSkill)
        {
            wildWindDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            MoveSlowPercent = (int)stats.MoveSlowPercent.FinalValue;
            AtkReducePercent = (int)stats.AtkReducePercent.FinalValue;
        }

        public override string Name() => wildWindDefinition.DisplayName;
        public override string Description() =>
            wildWindDefinition.DescribeEffect(resonantCellCount);

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