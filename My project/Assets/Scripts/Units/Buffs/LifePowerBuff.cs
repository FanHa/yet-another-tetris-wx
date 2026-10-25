using Model;
using Model.Buffs;
using Presentation;
using UnityEngine;

namespace Units.Buffs
{
    public class LifePowerBuff : Buff, IAttackHitTrigger
    {
        private readonly LifePowerBuffDefinition lifePowerDefinition;
        private readonly AdditiveValueBreakdown maxHealthConversionPercent;
        private readonly float attackHitBonusDamage;

        public LifePowerBuff(
            LifePowerBuffDefinition definition,
            float casterMaxHealthAtCast,
            int lifeResonanceCount,
            BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            lifePowerDefinition = definition;
            maxHealthConversionPercent = definition.CalculateCasterMaxHealthConversionPercent(lifeResonanceCount);
            attackHitBonusDamage = definition.CalculateAttackHitBonusDamage(
                casterMaxHealthAtCast,
                maxHealthConversionPercent.FinalValue
            );
        }

        public override string Name() => lifePowerDefinition.DisplayName;
        public override string DescribeAppliedEffect() => BuffDescriptionFormatter.Format(
            lifePowerDefinition.Description,
            DescribeEffect()
        );

        public string DescribeEffect() {
            return BuffDescriptionFormatter.Format(
                StatBreakdownFormatter.Format(
                    StatLabel.MaxHealthConversionPercent,
                    maxHealthConversionPercent),
                lifePowerDefinition.BaseDuration.ToDisplayString("效果持续时间"),
                StatBreakdownFormatter.Format(
                    StatLabel.AttackHitExtraDamage,
                    new AdditiveValueBreakdown(attackHitBonusDamage))
            );
        }
        // 攻击时触发，增加伤害
        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var extraDamage = new Damages.Damage(attackHitBonusDamage, Damages.DamageType.Skill)
                .SetSourceUnit(attacker)
                .SetTargetUnit(target)
                .SetSourceLabel(Name())
                .SetValue(attackHitBonusDamage);
            context.DealDamageTo(target, extraDamage);
        }
    }
}