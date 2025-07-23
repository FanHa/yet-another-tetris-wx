using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public class LifePowerBuff : Buff, IAttackHitTrigger
    {
        private float atkBoost;      // 攻击力加成

        public LifePowerBuff(float atkBoost, float duration, Unit sourceUnit, Skill sourceSkill)
            : base(duration, sourceUnit, sourceSkill)
        {
            this.atkBoost = atkBoost;
        }

        public override string Name() => "生命之力";
        public override string Description() => $"攻击时额外造成{atkBoost}伤害";

        // 攻击时触发，增加伤害
        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var extraDamage = new Damages.Damage(atkBoost,Damages.DamageType.Skill)
                .SetSourceUnit(attacker)
                .SetTargetUnit(target)
                .SetSourceLabel(Name())
                .SetValue(atkBoost);
            target.TakeDamage(extraDamage);
        }
    }
}