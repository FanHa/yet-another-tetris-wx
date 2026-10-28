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
        private Vector3 targetPosition;
        private readonly HashSet<Units.Projectiles.FrostZone> spawnedZones = new();

        public FrostZone(FrostZoneSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Context.TryGetClosestEnemyInAttackRange(out var targetEnemy))
                return false;
            targetPosition = targetEnemy.transform.position;
            return true;
        }
        protected override void ExecuteCore()
        {
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);
            var stats = GetStatsForDisplay(resonantCellCount);
            spawnedZones.RemoveWhere(zone => zone == null);

            var frostZone = Object.Instantiate(
                skillDefinition.ZonePrefab,
                targetPosition,
                Quaternion.identity
            );
            spawnedZones.Add(frostZone);
            frostZone.Initialize(
                caster: Context.Unit,
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
            var resonantCellCount = Context.GetResonanceCount(Definition.Resonance);
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