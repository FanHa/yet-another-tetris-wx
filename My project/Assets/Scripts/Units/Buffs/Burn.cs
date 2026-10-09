using Units.Skills;

namespace Units.Buffs
{
    public readonly struct BurnBuffArgs : IBuffCreationArgs
    {
        public float Dps { get; }
        public float Duration { get; }
        public BuffSource Source { get; }

        public BurnBuffArgs(float dps, float duration, BuffSource source)
        {
            Dps = dps;
            Duration = duration;
            Source = source;
        }
    }

    /// <summary>
    /// Burn：灼烧持续伤害Buff（Dot）
    /// </summary>
    public class Burn : Buff, ITick
    {
        private readonly float dps;
        private const string label = "灼烧";

        public Burn(Model.Buffs.BuffDefinition definition, BurnBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            dps = args.Dps;
        }

        public override string Name() => label;
        public override string GetKey()
        {
            return base.GetKey() + sourceSkill + sourceUnit;
        }
        public override string Description() => $"每秒造成{dps}点火焰伤害";

        public void OnTick(IBuffContext context)
        {
            var damage = new Damages.Damage(dps, Damages.DamageType.Skill);
            damage.SetSourceUnit(sourceUnit);
            damage.SetSourceLabel(sourceSkill.Name() + "-" +label);
            damage.SetTargetUnit(context.SelfUnit);
            context.TakeDamage(damage);
        }
    }
}