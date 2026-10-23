using Model.Buffs;

namespace Units.Buffs
{
    public class LifeEchoBuff : Buff, IAfterTakeDamageTrigger
    {
        private readonly LifeEchoBuffDefinition lifeEchoDefinition;
        private readonly int resonantCellCount;
        private readonly float reflectPercent;

        public LifeEchoBuff(LifeEchoBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            lifeEchoDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            reflectPercent = definition.CalculateStats(resonantCellCount).ReflectPercent.FinalValue;
        }

        public override string Name() => lifeEchoDefinition.DisplayName;

        public override string Description() => lifeEchoDefinition.DescribeEffect(resonantCellCount);

        public void OnAfterTakeDamage(IBuffContext context, ref Damages.Damage damage)
        {
            if (damage.Type == Damages.DamageType.Hit || damage.Type == Damages.DamageType.Skill)
            {
                // 反弹伤害给来源Unit
                var attacker = damage.SourceUnit;
                if (attacker != null && attacker != context.SelfUnit)
                {
                    float reflectValue = damage.Value * (reflectPercent / 100f);
                    var reflectDamage = new Damages.Damage(reflectValue, Damages.DamageType.Reflect)
                        .SetSourceUnit(context.SelfUnit)
                        .SetTargetUnit(attacker)
                        .SetSourceLabel("生命回响·反弹");
                    context.DealDamageTo(attacker, reflectDamage);
                }
            }
        }
    }
}