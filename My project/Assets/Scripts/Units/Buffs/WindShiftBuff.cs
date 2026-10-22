using Model.Buffs;

namespace Units.Buffs
{
    public class WindShiftBuff : Buff, IAttackHitTrigger
    {
        private readonly WindShiftBuffDefinition windShiftDefinition;
        private readonly int resonantCellCount;
        private readonly float attackRangeBonus;      // 攻击距离提升）

        public WindShiftBuff(WindShiftBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.Duration, source.SourceUnit, source.SourceSkill)
        {
            windShiftDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            attackRangeBonus = definition.CalculateStats(resonantCellCount).AttackRangeBonus.Final;
        }

        public override string Name() => windShiftDefinition.DisplayName;
        public override string Description() =>
            windShiftDefinition.DescribeEffect(resonantCellCount);


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