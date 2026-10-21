using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// Burn：灼烧持续伤害Buff（Dot）
    /// </summary>
    public class Burn : Buff, ITick
    {
        private readonly BurnBuffDefinition burnDefinition;
        private readonly float dps;

        public Burn(BurnBuffDefinition definition, float dps, float duration, BuffSource source)
            : base(definition, duration, source.SourceUnit, source.SourceSkill)
        {
            burnDefinition = definition;
            this.dps = dps;
        }

        public override string Name() => burnDefinition.DisplayName;
        public override string GetKey()
        {
            return Name() + sourceSkill.Name() + sourceUnit;
        }
        public override string Description() => burnDefinition.DescribeEffect(dps);

        public void OnTick(IBuffContext context)
        {
            var damage = new Damages.Damage(dps, Damages.DamageType.Skill);
            damage.SetSourceUnit(sourceUnit);
            damage.SetSourceLabel(sourceSkill.Name() + "-" + Name());
            damage.SetTargetUnit(context.SelfUnit);
            context.TakeDamage(damage);
        }
    }
}