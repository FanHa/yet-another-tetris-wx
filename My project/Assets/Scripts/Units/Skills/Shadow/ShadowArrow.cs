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

            // 创建并发射暗影箭
            var projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );

            projectile.Init(
                caster: Context.Unit,
                target: targetEnemy,
                sourceSkill: this,
                resonantCellCount: Context.GetResonanceCount(Definition.Resonance)
            );
            projectile.Activate();
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Damage}\n" +
                SkillConfig.ProjectilePrefab.DescribeEffect(Context.GetResonanceCount(Definition.Resonance)) + "\n";
        }

        public static string DescriptionStatic() => "向最大生命值最低的敌人发射暗影箭,施加易伤Debuff并造成伤害";

        public override string Name() => NameStatic();
        public static string NameStatic() => "暗影箭";
    }

}