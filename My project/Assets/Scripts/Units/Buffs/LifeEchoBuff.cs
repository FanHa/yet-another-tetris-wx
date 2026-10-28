using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    public class LifeEchoBuff : Buff, IAfterTakeDamageTrigger
    {
        private readonly LifeEchoBuffDefinition lifeEchoDefinition;
        private readonly AdditiveValueBreakdown reflectPercentBreakdown;
        private readonly float reflectPercent;

        public LifeEchoBuff(LifeEchoBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            lifeEchoDefinition = definition;
            reflectPercentBreakdown = definition.CalculateStats(resonantCellCount);
            reflectPercent = reflectPercentBreakdown.FinalValue;
        }

        public override string Name() => lifeEchoDefinition.DisplayName;

        public override string DescribeAppliedEffect() =>
            $"{Definition.Description}\n" +
            StatBreakdownFormatter.Format(StatLabel.ReflectedDamagePercent, reflectPercentBreakdown) + "\n" +
            Duration.ToDisplayString("效果持续时间");

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