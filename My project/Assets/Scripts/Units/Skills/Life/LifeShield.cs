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

        public LifeShield(LifeShieldSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        private (StatBreakdownDisplay LifeCostPercent, StatBreakdownDisplay ShieldMultiplierPercent, string Duration, float LifeCost, float ShieldValue) GetStatsForDisplay()
        {
            var stats = skillDefinition.CalculateStats(
                Owner.Attributes.CurrentHealth,
                Owner.GetResonanceCount(Definition.Resonance));
            return (
                new StatBreakdownDisplay(StatLabel.LifeCostPercent, stats.LifeCostPercent.BaseValue, stats.LifeCostPercent.AddedAmount),
                new StatBreakdownDisplay(StatLabel.ShieldMultiplierPercent, stats.ShieldMultiplierPercent.BaseValue, stats.ShieldMultiplierPercent.AddedAmount),
                stats.Duration.ToDisplayString("护盾持续时间"),
                stats.LifeCost,
                stats.ShieldValue);
        }

        protected override void ExecuteCore()
        {
            Unit targetAlly = UnitManager.FindRandomAlly(Owner);
            if (targetAlly == null)
            {
                return;
            }

            var stats = GetStatsForDisplay();

            var buff = new Units.Buffs.LifeShieldBuff(
                skillDefinition.LifeShieldBuffDefinition,
                stats.ShieldValue,
                skillDefinition.LifeShieldBuffDefinition.BaseDuration,
                new Units.Buffs.BuffSource(Owner, this)
            );
            var projectile = Object.Instantiate(
                skillDefinition.BuffProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            projectile.Init(Owner, targetAlly, buff);
            projectile.Activate();

            var damage = new Damages.Damage(stats.LifeCost, Damages.DamageType.Skill);
            damage.SetSourceUnit(Owner);
            damage.SetTargetUnit(Owner);
            damage.SetSourceLabel(Name());
            Owner.TakeDamage(damage);

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