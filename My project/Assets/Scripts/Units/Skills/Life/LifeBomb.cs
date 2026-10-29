using System.Linq;
using Model;
using Presentation;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class LifeBomb : ActiveSkill
    {
        private readonly LifeBombSkillDefinition skillDefinition;
        private Vector3 cachedTargetPos; 

        public LifeBomb(LifeBombSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        private StatBreakdownDisplay GetHealthCostDisplay(float currentHealth)
        {
            var stats = skillDefinition.CalculateStats(currentHealth);
            return new StatBreakdownDisplay(StatLabel.HealthCostPercent,
                stats.HealthCostPercent.BaseValue, stats.HealthCostPercent.AddedAmount);
        }

        protected override bool PrepareCastCore()
        {
            cachedTargetPos = default;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var targetEnemy))
                return false;

            cachedTargetPos = targetEnemy.transform.position;
            return true;
        }

        protected override void ExecuteCore()
        {
            var stats = skillDefinition.CalculateStats(Owner.Attributes.CurrentHealth);
            var tempTarget = Object.Instantiate(
                skillDefinition.TemporaryTargetPrefab,
                cachedTargetPos,
                Quaternion.identity
            );
            tempTarget.name = "临时目标 By " + Owner.name + " " + Name();

            // 投射炸弹
            Units.Projectiles.LifeBomb lifeBomb = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );

            lifeBomb.Init(
                caster: Owner,
                temporaryTarget: tempTarget,
                healthAmount: stats.HealthCost, // 伤害为消耗的生命值
                sourceSkill: this
            );
            
        }

        public override string DescribeEffect()
        {
            return Definition.Description + "\n" + GetHealthCostDisplay(Owner.Attributes.CurrentHealth);
        }

        public override string Name() => Definition.DisplayName;
    }
}