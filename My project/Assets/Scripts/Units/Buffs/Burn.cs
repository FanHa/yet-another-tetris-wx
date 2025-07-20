using Units.Skills;

namespace Units.Buffs
{
    /// <summary>
    /// Burn：灼烧持续伤害Buff（Dot）
    /// </summary>
    public class Burn : Buff, ITick
    {
        private readonly float dps;
        private const string label = "灼烧";

        public Burn(
            float dps,
            float duration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
            this.dps = dps;
        }

        public override string Name() => label;
        public override string GetKey()
        {
            return base.GetKey() + sourceSkill + sourceUnit;
        }
        public override string Description() => $"每秒造成{dps}点火焰伤害";

        public void OnTick(Unit unit)
        {
            var damage = new Damages.Damage(dps, Damages.DamageType.Skill);
            damage.SetSourceUnit(sourceUnit);
            damage.SetSourceLabel(sourceSkill.Name() + "-" +label);
            damage.SetTargetUnit(unit);
            unit.TakeDamage(damage);
        }
    }
}