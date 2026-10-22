using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// ShadowAttackBuff：攻击时对目标附加易伤Debuff
    /// </summary>
    public class ShadowAttackBuff : Buff, IAttackHitTrigger
    {
        private readonly ShadowAttackBuffDefinition shadowAttackDefinition;
        private readonly VulnerabilityBuffDefinition vulnerabilityDefinition;
        private readonly float vulnerabilityPercent;
        private readonly float vulnerabilityDuration;

        public ShadowAttackBuff(ShadowAttackBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.Duration, source.SourceUnit, source.SourceSkill)
        {
            shadowAttackDefinition = definition;
            vulnerabilityDefinition = definition.VulnerabilityDefinition;
            var stats = vulnerabilityDefinition.CalculateStats(resonantCellCount);
            vulnerabilityPercent = stats.ExtraDamagePercent.Final;
            vulnerabilityDuration = stats.Duration.Final;
        }

        public override string Name() => shadowAttackDefinition.DisplayName;
        public override string Description() =>
            shadowAttackDefinition.DescribeEffect(vulnerabilityPercent, vulnerabilityDuration);

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var vulnerability = new Vulnerability(
                vulnerabilityDefinition,
                extraDamagePercent: vulnerabilityPercent,
                buffDuration: vulnerabilityDuration,
                source: new BuffSource(attacker, sourceSkill)
            );
            context.AddBuffTo(target, vulnerability);
        }
    }
}