using Model;
using Presentation;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ThunderStrike : ActiveSkill
    {
        private readonly ThunderStrikeSkillDefinition skillDefinition;

        public ThunderStrike(ThunderStrikeSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override void ExecuteCore()
        {
            Unit targetEnemy = UnitManager.FindClosestEnemy(Owner);
            if (targetEnemy == null)
            {
                return;
            }

            var stats = CalcStats();

            Units.Projectiles.ThunderStrikeProjectile thunderStrikeEffect = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                targetEnemy.transform.position,
                Quaternion.identity
            );

            thunderStrikeEffect.Init(
                Owner,
                targetEnemy,
                stats.Damage.FinalValue,
                stats.EnergyDisruptionAmount,
                this
            );
            thunderStrikeEffect.Activate();

        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                Definition.Description + "\n" +
                $"{stats.Damage}";
        }

        public override string Name() => Definition.DisplayName;

        private struct ThunderStrikeStats
        {
            public StatBreakdownDisplay Damage;
            public float EnergyDisruptionAmount;
        }

        private ThunderStrikeStats CalcStats()
        {
            var stats = skillDefinition.CalculateStats(Owner.GetResonanceCount(Definition.Resonance));

            return new ThunderStrikeStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, stats.Damage.BaseValue, stats.Damage.AddedAmount),
                EnergyDisruptionAmount = stats.EnergyDisruptionAmount
            };
        }

    }
}
