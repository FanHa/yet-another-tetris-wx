using System.Linq;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// IceBreaker：攻击命中时，若目标带有 Chilled，清除该效果并按层数造成额外伤害
    /// 额外伤害 = stacks * extraPerStack
    /// </summary>
    public class IceBreaker : Buff, IAttackHitTrigger
    {
        private readonly float baseExtraDamage;
        private readonly float percentSlowMultiplier;

        public IceBreaker(
            float baseExtraDamage,
            float percentSlowMultiplier,
            float buffDuration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(buffDuration, sourceUnit, sourceSkill)
        {
            this.baseExtraDamage = baseExtraDamage;
            this.percentSlowMultiplier = percentSlowMultiplier;
        }
        public override string Name() => "破冰";
        public override string Description() =>
            $"命中时移除目标全部 Chilled。额外伤害 = {baseExtraDamage} + {percentSlowMultiplier} * (减速百分比)";


        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            if (target == null) return;

            var chilledBuffs = target.GetActiveBuffs()
                                     .OfType<Chilled>()
                                     .ToList();
            if (chilledBuffs.Count == 0) return;

            int totalPercentSlow = 0;
            foreach (Chilled buff in chilledBuffs)
            {
                totalPercentSlow += (buff.MoveSlowPercent + buff.AttackSlowPercent + buff.EnergyRegenSlowPercent) / 3;
                target.RemoveBuff(buff);
            }

            float extraDamage = baseExtraDamage + percentSlowMultiplier * totalPercentSlow;

            var bonus = new Damages.Damage(extraDamage, Damages.DamageType.Extra);
            bonus.SetSourceUnit(attacker);
            bonus.SetTargetUnit(target);
            bonus.SetSourceLabel("破冰加成");

            target.TakeDamage(bonus);
        }
    }
}