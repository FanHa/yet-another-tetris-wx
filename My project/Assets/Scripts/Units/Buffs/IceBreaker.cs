using System.Linq;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public readonly struct IceBreakerBuffArgs : IBuffCreationArgs
    {
        public float BaseExtraDamage { get; }
        public float PercentSlowMultiplier { get; }
        public float BuffDuration { get; }
        public BuffSource Source { get; }

        public IceBreakerBuffArgs(float baseExtraDamage, float percentSlowMultiplier, float buffDuration, BuffSource source)
        {
            BaseExtraDamage = baseExtraDamage;
            PercentSlowMultiplier = percentSlowMultiplier;
            BuffDuration = buffDuration;
            Source = source;
        }
    }

    /// <summary>
    /// IceBreaker：攻击命中时，若目标带有 Chilled，清除该效果并按层数造成额外伤害
    /// 额外伤害 = stacks * extraPerStack
    /// </summary>
    public class IceBreaker : Buff, IAttackHitTrigger
    {
        private readonly float baseExtraDamage;
        private readonly float percentSlowMultiplier;

        public IceBreaker(Model.Buffs.BuffDefinition definition, IceBreakerBuffArgs args)
            : base(definition, args.BuffDuration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            baseExtraDamage = args.BaseExtraDamage;
            percentSlowMultiplier = args.PercentSlowMultiplier;
        }
        public override string Name() => "破冰";
        public override string Description() =>
            $"命中时移除目标全部 Chilled。额外伤害 = {baseExtraDamage} + {percentSlowMultiplier} * (减速百分比)";


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
                totalPercentSlow += (buff.MoveSlowPercent + buff.AttackSlowPercent + buff.EnergyRegenSlowPercent) / 3;
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