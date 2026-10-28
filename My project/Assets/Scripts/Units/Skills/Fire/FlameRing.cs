using Model;
using Presentation;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class FlameRing : Skill
    {
        private readonly FlameRingSkillDefinition skillDefinition;
        private Units.Projectiles.FlameRing areaEffect;

        public FlameRing(FlameRingSkillDefinition definition)
        {
            skillDefinition = definition;
        }

        private (StatBreakdownDisplay Radius, float AreaDuration) GetStatsForDisplay()
        {
            int resonantCellCount = OwnerContext.GetResonanceCount(Definition.Resonance);
            var stats = skillDefinition.CalculateStats(resonantCellCount);
            return (
                new StatBreakdownDisplay(StatLabel.ActionRadius, stats.Radius.BaseValue, stats.Radius.AddedAmount),
                stats.AreaDuration);
        }

        public override string DescribeEffect()
        {
            var stats = GetStatsForDisplay();
            return
                Definition.Description + ":\n" +
                skillDefinition.ZonePrefab.DescribeEffect(OwnerContext.GetResonanceCount(Definition.Resonance)) + "\n" +
                $"{stats.Radius}\n";
        }

        public override string Name() => Definition.DisplayName;

        public override void OnActivate()
        {
            var stats = GetStatsForDisplay();
            areaEffect = Object.Instantiate(
                skillDefinition.ZonePrefab,
                OwnerContext.Transform.position,
                Quaternion.identity
            );
            areaEffect.Initialize(
                owner: OwnerContext.Owner,
                radius: stats.Radius.FinalValue,
                sourceSkill: this,
                duration: stats.AreaDuration,
                resonantCellCount: OwnerContext.GetResonanceCount(Definition.Resonance)
            );
            areaEffect.Activate();
        }

        public override void OnDeactivate()
        {
            ClearAreaEffect();
        }

        public override void OnRelease()
        {
            ClearAreaEffect();
        }

        private void ClearAreaEffect()
        {
            if (areaEffect == null)
            {
                return;
            }

            Object.Destroy(areaEffect.gameObject);
            areaEffect = null;
        }
    }
}