using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using Presentation;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class FrostZone : ActiveSkill
    {
        private readonly FrostZoneSkillDefinition skillDefinition;
        private readonly HashSet<Units.Projectiles.FrostZone> spawnedZones = new();

        public FrostZone(FrostZoneSkillDefinition definition)
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

            Vector3 targetPosition = targetEnemy.transform.position;
            int resonantCellCount = Owner.GetResonanceCount(Definition.Resonance);
            var stats = GetStatsForDisplay(resonantCellCount);
            spawnedZones.RemoveWhere(zone => zone == null);

            var frostZone = Object.Instantiate(
                skillDefinition.ZonePrefab,
                targetPosition,
                Quaternion.identity
            );
            spawnedZones.Add(frostZone);
            frostZone.Initialize(
                caster: Owner,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                damage: stats.Damage.FinalValue,
                sourceSkill: this,
                resonantCellCount: resonantCellCount
            );
            frostZone.Activate();
        }

        public override void OnRelease()
        {
            foreach (var zone in spawnedZones)
            {
                if (zone != null)
                {
                    Object.Destroy(zone.gameObject);
                }
            }

            spawnedZones.Clear();
        }


        public override string DescribeEffect()
        {
            var resonantCellCount = Owner.GetResonanceCount(Definition.Resonance);
            var stats = GetStatsForDisplay(resonantCellCount);
            return
                Definition.Description + ":\n" +
                $"{stats.Duration}\n" +
                $"{stats.Radius}\n" +
                $"{stats.Damage}\n" +
                skillDefinition.ZonePrefab.DescribeEffect(resonantCellCount);
        }

        public override string Name() => Definition.DisplayName;

        private struct FrostZoneStats
        {
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay Duration;
            public StatBreakdownDisplay Damage;
        }

        private FrostZoneStats GetStatsForDisplay(int resonantCellCount)
        {
            var stats = skillDefinition.CalculateStats(resonantCellCount);
            return new FrostZoneStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.AreaRadius, stats.Radius.BaseValue, stats.Radius.AddedAmount),
                Duration = new StatBreakdownDisplay(StatLabel.Duration, stats.Duration.BaseValue, stats.Duration.AddedAmount),
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, stats.Damage.BaseValue, stats.Damage.AddedAmount)
            };
        }
    }
}