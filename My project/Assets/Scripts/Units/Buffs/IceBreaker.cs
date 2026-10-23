using System.Linq;
using Model.Buffs;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// IceBreaker：攻击命中时，若目标带有 Chilled，清除该效果并按层数造成额外伤害
    /// 额外伤害 = stacks * extraPerStack
    /// </summary>
    public class IceBreaker : Buff, IAttackHitTrigger
    {
        private readonly IceBreakerBuffDefinition iceBreakerDefinition;
        private readonly int resonantCellCount;
        private readonly float baseExtraDamage;
        private readonly float percentSlowMultiplier;

        public IceBreaker(IceBreakerBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            iceBreakerDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            var stats = definition.CalculateStats(resonantCellCount);
            baseExtraDamage = stats.BaseExtraDamage.FinalValue;
            percentSlowMultiplier = stats.PercentSlowMultiplier.FinalValue;
        }
        public override string Name() => iceBreakerDefinition.DisplayName;
        public override string Description() =>
            iceBreakerDefinition.DescribeEffect(resonantCellCount);


        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            if (target == null) return;

            var chilledBuffs = target.GetActiveBuffsReadOnly()
                                     .OfType<Chilled>()
                                     .ToList();
            if (chilledBuffs.Count == 0) return;

            int totalPercentSlow = 0;
            foreach (Chilled buff in chilledBuffs)
            {
                totalPercentSlow += (buff.MoveSlowPercent + buff.AttackSlowPercent + buff.ActionSlowPercent) / 3;
                context.RemoveBuffFrom(target, buff);
            }

            float extraDamage = baseExtraDamage + percentSlowMultiplier * totalPercentSlow;

            var bonus = new Damages.Damage(extraDamage, Damages.DamageType.Extra);
            bonus.SetSourceUnit(attacker);
            bonus.SetTargetUnit(target);
            bonus.SetSourceLabel("破冰加成");

            context.DealDamageTo(target, bonus);
        }
    }
}