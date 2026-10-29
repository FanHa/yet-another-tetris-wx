using Model;
using Presentation;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ChainLightning : ActiveSkill
    {
        private readonly ChainLightningSkillDefinition skillDefinition;
        private Unit initialTarget;

        public ChainLightning(ChainLightningSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        public override string DescribeEffect()
        {
            var stats = GetStatsForDisplay();
            return Definition.Description + ":\n" +
                   $"{stats.Damage}\n" +
                   $"每次弹射伤害增加 {skillDefinition.DamageIncreasePercentage}%，最多弹射 {skillDefinition.MaxBounces} 次。";
        }

        protected override bool PrepareCastCore()
        {
            initialTarget = null;
            initialTarget = Owner.UnitManager.FindClosestEnemyInRange(Owner, skillDefinition.Range);
            return initialTarget != null;
        }

        protected override void ExecuteCore()
        {
            Projectiles.ChainLightning projectile = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            var stats = GetStatsForDisplay();
            projectile.Initialize(
                caster: Owner,
                initialTarget: initialTarget,
                baseDamage: stats.Damage.FinalValue,
                damageIncreasePercentage: skillDefinition.DamageIncreasePercentage,
                maxTargets: skillDefinition.MaxBounces + 1,
                chainRange: skillDefinition.Range,
                energyDisruptionAmount: stats.EnergyDisruptionAmount,
                sourceSkill: this
            );
            projectile.Activate();
            initialTarget = null;
        }

        private (StatBreakdownDisplay Damage, float EnergyDisruptionAmount) GetStatsForDisplay()
        {
            var stats = skillDefinition.CalculateStats(Owner.GetResonanceCount(Definition.Resonance));
            return (
                new StatBreakdownDisplay(StatLabel.FirstChainDamage, stats.InitialDamage.BaseValue, stats.InitialDamage.AddedAmount),
                stats.EnergyDisruptionAmount);
        }

        public override string Name() => Definition.DisplayName;
    }
}