using Model;
using Model.Buffs;
using Presentation;

namespace Units.Buffs
{
    public class WindShiftBuff : Buff, IAttackHitTrigger
    {
        private readonly WindShiftBuffDefinition windShiftDefinition;
        private readonly AdditiveValueBreakdown attackRangeBonusStats;
        private readonly float attackRangeBonus;      // 攻击距离提升）

        public WindShiftBuff(WindShiftBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            windShiftDefinition = definition;
            var stats = definition.CalculateStats(resonantCellCount);
            attackRangeBonusStats = stats.AttackRangeBonus;
            attackRangeBonus = stats.AttackRangeBonus.FinalValue;
        }

        public override string Name() => windShiftDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"攻击时额外造成等于当前最终攻击距离的伤害。\n" +
            $"{StatBreakdownFormatter.Format(StatLabel.AttackRangeIncrease, attackRangeBonusStats)}\n" +
            Duration.ToDisplayString("风形态持续时间");


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
            var windDamage = new Damages.Damage(extraDamage, Damages.DamageType.Skill);
            windDamage.SetSourceUnit(attacker);
            windDamage.SetTargetUnit(target);
            windDamage.SetSourceLabel("风形态加成");

            context.DealDamageTo(target, windDamage);
        }
    }
}