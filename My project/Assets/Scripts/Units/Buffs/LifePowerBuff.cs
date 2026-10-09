using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public readonly struct LifePowerBuffArgs : IBuffCreationArgs
    {
        public float AtkBoost { get; }
        public float Duration { get; }
        public BuffSource Source { get; }

        public LifePowerBuffArgs(float atkBoost, float duration, BuffSource source)
        {
            AtkBoost = atkBoost;
            Duration = duration;
            Source = source;
        }
    }

    public class LifePowerBuff : Buff, IAttackHitTrigger
    {
        private float atkBoost;      // 攻击力加成

        public LifePowerBuff(Model.Buffs.BuffDefinition definition, LifePowerBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            atkBoost = args.AtkBoost;
        }

        public override string Name() => "生命之力";
        public override string Description() => $"攻击时额外造成{atkBoost}伤害";

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