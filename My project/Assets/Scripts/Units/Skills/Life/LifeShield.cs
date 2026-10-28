using System.Linq;
using Model;
using Presentation;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class LifeShield : ActiveSkill
    {
        private readonly LifeShieldSkillDefinition skillDefinition;
        private Unit cachedTarget;

        public LifeShield(LifeShieldSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        private (StatBreakdownDisplay LifeCostPercent, StatBreakdownDisplay ShieldMultiplierPercent, string Duration, float LifeCost, float ShieldValue) GetStatsForDisplay()
        {
            var stats = skillDefinition.CalculateStats(
                Context.Attributes.CurrentHealth,
                Context.GetResonanceCount(Definition.Resonance));
            return (
                new StatBreakdownDisplay(StatLabel.LifeCostPercent, stats.LifeCostPercent.BaseValue, stats.LifeCostPercent.AddedAmount),
                new StatBreakdownDisplay(StatLabel.ShieldMultiplierPercent, stats.ShieldMultiplierPercent.BaseValue, stats.ShieldMultiplierPercent.AddedAmount),
                stats.Duration.ToDisplayString("护盾持续时间"),
                stats.LifeCost,
                stats.ShieldValue);
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            // 找到友方目标（不包括自己）
            cachedTarget = Context.FindRandomAlly();
            if (cachedTarget == null)
                return false;

            return true;
        }

        protected override void ExecuteCore()
        {
            if (cachedTarget == null)
            return;

            var stats = GetStatsForDisplay();

            var buff = new Units.Buffs.LifeShieldBuff(
                skillDefinition.LifeShieldBuffDefinition,
                stats.ShieldValue,
                skillDefinition.LifeShieldBuffDefinition.BaseDuration,
                new Units.Buffs.BuffSource(Context.Unit, this)
            );
            var projectile = Object.Instantiate(
                skillDefinition.BuffProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );
            projectile.Init(Context.Unit, cachedTarget, buff);
            projectile.Activate();

            var damage = new Damages.Damage(stats.LifeCost, Damages.DamageType.Skill);
            damage.SetSourceUnit(Context.Unit);
            damage.SetTargetUnit(Context.Unit);
            damage.SetSourceLabel(Name());
            Context.Unit.TakeDamage(damage);

            cachedTarget = null;
        }

        public override string DescribeEffect()
        {
            var stats = GetStatsForDisplay();
            return
                Definition.Description + "\n" +
                $"{stats.LifeCostPercent}\n{stats.ShieldMultiplierPercent}\n{stats.Duration}";
        }

        public override string Name() => Definition.DisplayName;
    }
}