using Units.Skills;

namespace Units.Buffs
{
    public readonly struct WindShiftBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public BuffSource Source { get; }
        public float AttackRangeBonus { get; }

        public WindShiftBuffArgs(float duration, float attackRangeBonus, BuffSource source)
        {
            Duration = duration;
            Source = source;
            AttackRangeBonus = attackRangeBonus;
        }
    }

    public class WindShiftBuff : Buff, IAttackHitTrigger
    {
        private readonly float attackRangeBonus;      // 攻击距离提升）

        public WindShiftBuff(Model.Buffs.BuffDefinition definition, WindShiftBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            attackRangeBonus = args.AttackRangeBonus;
        }

        public override string Name() => "风形态";
        public override string Description() =>
            $"攻击距离+{attackRangeBonus}，攻击时获得额外的与攻击距离相关的伤害加成";


        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttackRange.AddFlatModifier(this, attackRangeBonus);
        }


        public override void OnRemove()
        {
            context.Attributes.AttackRange.RemoveFlatModifier(this);
            base.OnRemove();
        }

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            // 附加伤害与当前攻击距离相关（可配合其他攻击距离加成机制）
            float extraDamage = attacker.Attributes.AttackRange.finalValue;
            var windDamage = new Damages.Damage(extraDamage, Damages.DamageType.Extra);
            windDamage.SetSourceUnit(attacker);
            windDamage.SetTargetUnit(target);
            windDamage.SetSourceLabel("风形态加成");

            context.DealDamageTo(target, windDamage);
        }
    }
}