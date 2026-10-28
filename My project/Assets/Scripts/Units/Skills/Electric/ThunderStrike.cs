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
        private Unit cachedTarget;

        public ThunderStrike(ThunderStrikeSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            if (!Context.TryGetClosestEnemyInAttackRange(out var found))
                return false;

            cachedTarget = found;
            return true;
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            Units.Projectiles.ThunderStrikeProjectile thunderStrikeEffect = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                cachedTarget.transform.position,
                Quaternion.identity
            );

            thunderStrikeEffect.Init(
                Context.Unit,
                cachedTarget,
                stats.Damage.FinalValue,
                stats.EnergyDisruptionAmount,
                this
            );
            thunderStrikeEffect.Activate();

            cachedTarget = null;
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
            var stats = skillDefinition.CalculateStats(Context.GetResonanceCount(Definition.Resonance));

            return new ThunderStrikeStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, stats.Damage.BaseValue, stats.Damage.AddedAmount),
                EnergyDisruptionAmount = stats.EnergyDisruptionAmount
            };
        }

    }
}
