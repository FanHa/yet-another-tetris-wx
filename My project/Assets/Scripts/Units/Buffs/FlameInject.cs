using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// FlameInject Buff：攻击时对目标附加火焰伤害并施加灼烧Dot
    /// </summary>
    public class FlameInject : Buff, IAttackHitTrigger
    {
        private readonly float dotDps;
        private readonly float dotDuration;
        private readonly BurnBuffDefinition burnDefinition;

        public FlameInject(FlameInjectBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            burnDefinition = definition.BurnDefinition;
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            dotDps = stats.Dps.FinalValue;
            dotDuration = stats.Duration.FinalValue;
        }

        public override string Name() => "炎附";
        public override string Description() =>
            $"攻击时对目标附加{dotDps}/s灼烧({dotDuration}秒)";

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var burn = new Burn(
                burnDefinition,
                dotDps,
                BuffDuration.ForSeconds(dotDuration),
                new BuffSource(attacker, sourceSkill)
            );
            context.AddBuffTo(target, burn);
        }
    }
}