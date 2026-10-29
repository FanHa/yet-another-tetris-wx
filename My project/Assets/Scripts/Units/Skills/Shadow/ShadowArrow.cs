using System.Linq;
using Model;
using Presentation;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowArrow : ActiveSkill
    {
        private readonly ShadowArrowSkillDefinition skillDefinition;

        public ShadowArrow(ShadowArrowSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }
        
        private struct ShadowArrowStats
        {
            public StatBreakdownDisplay Damage;
        }

        private ShadowArrowStats CalcStats()
        {
            int resonantCellCount = Owner.GetResonanceCount(Definition.Resonance);
            AdditiveValueBreakdown damage = skillDefinition.CalculateDamage(resonantCellCount);
            return new ShadowArrowStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, damage.BaseValue, damage.AddedAmount)
            };
        }
        protected override void ExecuteCore()
        {
            // 查找最大生命值最低的敌人
            Unit targetEnemy = Owner.UnitManager.GetActiveEnemies(Owner)
                .OrderBy(enemy => enemy.Attributes.MaxHealth.finalValue)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            var stats = CalcStats();

            // 创建并发射暗影箭
            var projectile = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );

            projectile.Init(
                caster: Owner,
                target: targetEnemy,
                damageValue: stats.Damage.FinalValue,
                sourceSkill: this,
                resonantCellCount: Owner.GetResonanceCount(Definition.Resonance)
            );
            projectile.Activate();
        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                Definition.Description + ":\n" +
                $"{stats.Damage}\n" +
                skillDefinition.ProjectilePrefab.DescribeEffect(Owner.GetResonanceCount(Definition.Resonance)) + "\n";
        }

        public override string Name() => Definition.DisplayName;
    }

}