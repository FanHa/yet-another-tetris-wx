using Model.Buffs;
using UnityEngine;

namespace Units.Buffs
{
    public class LifePowerBuff : Buff, IAttackHitTrigger
    {
        private readonly LifePowerBuffDefinition lifePowerDefinition;
        private readonly float attackHitBonusDamage;

        public LifePowerBuff(
            LifePowerBuffDefinition definition,
            float attackHitBonusDamage,
            BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            lifePowerDefinition = definition;
            this.attackHitBonusDamage = attackHitBonusDamage;
        }

        public override string Name() => lifePowerDefinition.DisplayName;
        public override string DescribeAppliedEffect() {
            return $"{lifePowerDefinition.Description}\n" + 
            $"{lifePowerDefinition.BaseDuration.ToDisplayString("效果持续时间")}\n" +
            $"攻击命中额外伤害: <final>{attackHitBonusDamage}</final>";
        }

        private string DescribeEffectDetails() =>
            $"{lifePowerDefinition.BaseDuration.ToDisplayString("效果持续时间")}\n" +
            $"攻击命中额外伤害: <final>{attackHitBonusDamage}</final>";

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