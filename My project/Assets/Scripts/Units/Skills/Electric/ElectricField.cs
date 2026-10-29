using System.Collections.Generic;
using Model;
using Presentation;
using Model.Skills;
using Model.Resonance;
using UnityEngine;

namespace Units.Skills
{
    public sealed class ElectricField : ActiveSkill
    {
        private readonly ElectricFieldSkillDefinition skillDefinition;
        private readonly HashSet<Units.Projectiles.ElectricFieldZone> spawnedZones = new();
        private Vector3 targetPosition;

        public ElectricField(ElectricFieldSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            if (!Owner.TryGetClosestEnemyInAttackRange(out Unit target))
            {
                return false;
            }

            targetPosition = target.transform.position;
            return true;
        }

        protected override void ExecuteCore()
        {
            ElectricFieldStats stats = GetStatsForDisplay();
            spawnedZones.RemoveWhere(zone => zone == null);

            Units.Projectiles.ElectricFieldZone zone = Object.Instantiate(
                skillDefinition.ZonePrefab,
                targetPosition,
                Quaternion.identity);
            spawnedZones.Add(zone);
            zone.Initialize(
                caster: Owner,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                damage: stats.Damage.FinalValue,
                energyDisruptionAmount: stats.EnergyDisruptionAmount.FinalValue,
                sourceSkill: this);
            zone.Activate();
        }

        public override void OnRelease()
        {
            foreach (Units.Projectiles.ElectricFieldZone zone in spawnedZones)
            {
                if (zone != null)
                {
                    Object.Destroy(zone.gameObject);
                }
            }

            spawnedZones.Clear();
        }

        public override string Name() => Definition.DisplayName;

        public override string DescribeEffect()
        {
            ElectricFieldStats stats = GetStatsForDisplay();
            return Definition.Description + "\n"
                   + $"{stats.Radius}\n"
                   + $"{stats.Duration}\n"
                   + $"触发间隔: <final>{skillDefinition.ZonePrefab.ApplyInterval}</final>\n"
                   + $"{stats.Damage}\n"
                   + $"{stats.EnergyDisruptionAmount}";
        }

        private ElectricFieldStats GetStatsForDisplay()
        {
            var stats = skillDefinition.CalculateStats(Owner.GetResonanceCount(Definition.Resonance));

            return new ElectricFieldStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.AreaRadius, stats.Radius.BaseValue, stats.Radius.AddedAmount),
                Duration = new StatBreakdownDisplay(StatLabel.Duration, stats.Duration.BaseValue, stats.Duration.AddedAmount),
                Damage = new StatBreakdownDisplay(StatLabel.ElectricDamage, stats.Damage.BaseValue, stats.Damage.AddedAmount),
                EnergyDisruptionAmount = new StatBreakdownDisplay(StatLabel.SkillEnergyReduction,
                    stats.EnergyDisruptionAmount.BaseValue, stats.EnergyDisruptionAmount.AddedAmount)
            };
        }

        private struct ElectricFieldStats
        {
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay Duration;
            public StatBreakdownDisplay Damage;
            public StatBreakdownDisplay EnergyDisruptionAmount;
        }
    }
}