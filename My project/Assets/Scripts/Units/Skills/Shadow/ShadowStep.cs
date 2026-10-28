using System.Linq;
using Model;
using Presentation;
using Model.Buffs;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowStep : ActiveSkill
    {
        private readonly ShadowStepSkillDefinition skillDefinition;

        public ShadowStep(ShadowStepSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
            InitializeInitialEnergy(definition.InitialEnergy);
        }

        private struct ShadowStepStats
        {
            public AdditiveValueBreakdown VulnerabilityPercent;
            public StatBreakdownDisplay Damage;
            public AdditiveValueBreakdown DebuffDuration;
        }

        private ShadowStepStats CalcStats()
        {
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);
            var vulnerabilityStats = skillDefinition.VulnerabilityBuffDefinition.CalculateStats(resonantCellCount);
            AdditiveValueBreakdown damage = skillDefinition.CalculateDamage(resonantCellCount);
            return new ShadowStepStats
            {
                VulnerabilityPercent = vulnerabilityStats.ExtraDamagePercent,
                Damage = new StatBreakdownDisplay(
                    StatLabel.SkillDamage,
                    damage.BaseValue,
                    damage.AddedAmount
                ),
                DebuffDuration = vulnerabilityStats.Duration
            };
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            Unit targetEnemy = Context.GetActiveEnemies()
                .OrderBy(enemy => enemy.Attributes.MaxHealth.finalValue)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            Vector3 dir = (targetEnemy.transform.position - Context.Transform.position).normalized;
            Vector3 targetPos = targetEnemy.transform.position + dir * 1.2f; // 1.2f为身后距离，可调整

            Context.Teleport(targetPos);

            var vulnerabilityBuff = new Units.Buffs.Vulnerability(
                skillDefinition.VulnerabilityBuffDefinition,
                extraDamagePercent: stats.VulnerabilityPercent.FinalValue,
                buffDuration: BuffDuration.ForSeconds(stats.DebuffDuration.FinalValue),
                source: new Units.Buffs.BuffSource(Context.Unit, this)
            );
            targetEnemy.AddBuff(vulnerabilityBuff);

            // 造成伤害
            var damage = new Damages.Damage(stats.Damage.FinalValue, Damages.DamageType.Hit);
            damage.SetSourceUnit(Context.Unit);
            damage.SetTargetUnit(targetEnemy);
            damage.SetSourceLabel(Name());
            targetEnemy.TakeDamage(damage);
            // todo 动画

        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                Definition.Description + ":\n" +
                $"{StatBreakdownFormatter.Format(StatLabel.VulnerabilityPercent, stats.VulnerabilityPercent)}\n" +
                $"{stats.Damage}\n" +
                $"{StatBreakdownFormatter.Format(StatLabel.VulnerabilityDuration, stats.DebuffDuration)}\n";
        }

        public override string Name() => Definition.DisplayName;
    }
}