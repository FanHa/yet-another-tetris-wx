using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    /// <summary>
    /// FlameInject Buff：攻击命中时对目标施加灼烧Dot
    /// </summary>
    public class FlameInject : Buff, IAttackHitTrigger
    {
        private readonly AdditiveValueBreakdown dotDps;
        private readonly AdditiveValueBreakdown dotDuration;
        private readonly BurnBuffDefinition burnDefinition;

        public FlameInject(FlameInjectBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            burnDefinition = definition.BurnDefinition;
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            dotDps = stats.Dps;
            dotDuration = stats.Duration;
        }

        public string DescribeEffect()
        {
            return string.Join("\n",
                StatBreakdownFormatter.Format(StatLabel.BurnDamagePerSecond, dotDps),
                $"{StatBreakdownFormatter.Format(StatLabel.BurnDuration, dotDuration)} 秒",
                DescribeDuration());
        }

        public override string Name() => Definition.DisplayName;
        public override string DescribeAppliedEffect() =>
            string.Join("\n",
                Definition.Description,
                $"灼烧每秒伤害: {dotDps.FinalValue}",
                $"灼烧持续时间: {dotDuration.FinalValue} 秒",
                DescribeDuration());

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var burn = new Burn(
                burnDefinition,
                dotDps.FinalValue,
                BuffDuration.ForSeconds(dotDuration.FinalValue),
                new BuffSource(attacker, sourceSkill)
            );
            context.AddBuffTo(target, burn);
        }
    }
}