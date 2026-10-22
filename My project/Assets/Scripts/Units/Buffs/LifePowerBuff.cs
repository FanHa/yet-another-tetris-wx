using Model.Buffs;
using UnityEngine;

namespace Units.Buffs
{
    public class LifePowerBuff : Buff, IAttackHitTrigger
    {
        private readonly LifePowerBuffDefinition lifePowerDefinition;
        private readonly int resonantCellCount;
        private readonly float atkBoost;

        public LifePowerBuff(LifePowerBuffDefinition definition, float casterMaxHealth, int resonantCellCount, BuffSource source)
            : base(definition, definition.Duration, source.SourceUnit, source.SourceSkill)
        {
            lifePowerDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            atkBoost = definition.CalculateExtraDamage(casterMaxHealth, resonantCellCount);
        }

        public override string Name() => lifePowerDefinition.DisplayName;
        public override string Description() => lifePowerDefinition.DescribeEffect(atkBoost, resonantCellCount);

        // 攻击时触发，增加伤害
        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var extraDamage = new Damages.Damage(atkBoost,Damages.DamageType.Skill)
                .SetSourceUnit(attacker)
                .SetTargetUnit(target)
                .SetSourceLabel(Name())
                .SetValue(atkBoost);
            context.DealDamageTo(target, extraDamage);
        }
    }
}