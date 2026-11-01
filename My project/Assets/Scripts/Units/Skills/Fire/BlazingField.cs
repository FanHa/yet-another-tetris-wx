using System.Collections.Generic;
using Model;
using Presentation;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class BlazingField : ActiveSkill
    {
        private readonly BlazingFieldSkillDefinition skillDefinition;
        private readonly HashSet<Units.Projectiles.BlazingField> spawnedFields = new();

        public BlazingField(BlazingFieldSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        private (StatBreakdownDisplay Radius, StatBreakdownDisplay Duration) GetStatsForDisplay()
        {
            int resonantCellCount = Owner.GetResonanceCount(Definition.Resonance);
            var stats = skillDefinition.CalculateStats(resonantCellCount);
            return (
                new StatBreakdownDisplay(StatLabel.AreaRadius, stats.Radius.BaseValue, stats.Radius.AddedAmount),
                new StatBreakdownDisplay(StatLabel.Duration, stats.Duration.BaseValue, stats.Duration.AddedAmount));
        }

        protected override void ExecuteCore()
        {
            Unit targetEnemy = UnitManager.FindClosestEnemy(Owner);
            if (targetEnemy == null)
            {
                return;
            }

            Vector3 targetPosition = targetEnemy.transform.position;
            var stats = GetStatsForDisplay();
            spawnedFields.RemoveWhere(field => field == null);

            var effect = Object.Instantiate(
                skillDefinition.ZonePrefab,
                targetPosition,
                Quaternion.identity
            );
            spawnedFields.Add(effect);
            effect.Init(
                caster: Owner,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                sourceSkill: this,
                resonantCellCount: Owner.GetResonanceCount(Definition.Resonance)
            );
            effect.Activate();
        }

        public override void OnRelease()
        {
            foreach (var effect in spawnedFields)
            {
                if (effect != null)
                {
                    Object.Destroy(effect.gameObject);
                }
            }

            spawnedFields.Clear();
        }


        public override string DescribeEffect()
        {
            var stats = GetStatsForDisplay();
            return
                Definition.Description + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.Duration}\n" +
                skillDefinition.ZonePrefab.DescribeEffect(Owner.GetResonanceCount(Definition.Resonance));
        }

        public override string Name() => Definition.DisplayName;
    }
}