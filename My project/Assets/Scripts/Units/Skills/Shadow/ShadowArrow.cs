using System.Linq;
using Model;
using Presentation;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowArrow : ActiveSkill
    {
        public ShadowArrowLevelConfig Config { get; }

        private ShadowArrowSkillConfig SkillConfig => (ShadowArrowSkillConfig)Definition.Config;

        public ShadowArrow(ShadowArrowLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }
        
        private struct ShadowArrowStats
        {
            public StatBreakdownDisplay Damage;
        }

        private ShadowArrowStats CalcStats()
        {
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);
            return new ShadowArrowStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, Config.Damage, resonantCellCount * Config.DamagePerResonantCell)
            };
        }
        protected override void ExecuteCore()
        {
            // 查找最大生命值最低的敌人
            Unit targetEnemy = Context.GetActiveEnemies()
                .OrderBy(enemy => enemy.Attributes.MaxHealth.finalValue)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            var stats = CalcStats();

            // 创建并发射暗影箭
            var projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );

            projectile.Init(
                caster: Context.Unit,
                target: targetEnemy,
                damageValue: stats.Damage.FinalValue,
                sourceSkill: this,
                resonantCellCount: Context.GetResonanceCount(Definition.Resonance)
            );
            projectile.Activate();
        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                Definition.Description + ":\n" +
                $"{stats.Damage}\n" +
                SkillConfig.ProjectilePrefab.DescribeEffect(Context.GetResonanceCount(Definition.Resonance)) + "\n";
        }

        public override string Name() => Definition.DisplayName;
    }

}