using Model;
using Presentation;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class WildWind : ActiveSkill
    {
        private readonly WildWindSkillDefinition skillDefinition;

        public WildWind(WildWindSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        private struct WildWindStats
        {
            public StatBreakdownDisplay Damage;
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay Duration;
        }

        private WildWindStats CalcStats()
        {
            int resonantCellCount = Owner.GetResonanceCount(Definition.Resonance);
            var stats = skillDefinition.CalculateStats(resonantCellCount);
            return new WildWindStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.Damage, stats.Damage.BaseValue, stats.Damage.AddedAmount),
                Radius = new StatBreakdownDisplay(StatLabel.ActionRadius, stats.Radius.BaseValue, stats.Radius.AddedAmount),
                Duration = new StatBreakdownDisplay(StatLabel.WindFieldDuration, stats.Duration.BaseValue, stats.Duration.AddedAmount)
            };
        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                Definition.Description + "\n" +
                $"{stats.Damage}\n" +
                $"{stats.Radius}\n" +
                $"{stats.Duration}\n" +
                skillDefinition.ProjectilePrefab.DescribeEffect(Owner.GetResonanceCount(Definition.Resonance));
        }

            public override string Name() => Definition.DisplayName;

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            var wildWind = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            wildWind.Initialize(
                caster: Owner,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                damage: stats.Damage.FinalValue,
                resonantCellCount: Owner.GetResonanceCount(Definition.Resonance),
                sourceSkill: this // 传递技能引用
            );
            wildWind.Activate();
        }
    }
}