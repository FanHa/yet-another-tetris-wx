using System.Linq;
using Model;
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

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }

        private ShadowArrowStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            return new ShadowArrowStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, Config.Damage, resonantCellCount * Config.DamagePerResonantCell)
            };
        }
        protected override void ExecuteCore()
        {
            // 查找最大生命值最低的敌人
            Unit targetEnemy = Owner.GetActiveEnemies()
                .OrderBy(enemy => enemy.Attributes.MaxHealth.finalValue)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            // 创建并发射暗影箭
            var projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );

            projectile.Init(
                caster: Owner.SelfUnit,
                target: targetEnemy,
                sourceSkill: this,
                resonantCellCount: GetResonantCellCount()
            );
            projectile.Activate();
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Damage}\n" +
                SkillConfig.ProjectilePrefab.DescribeEffect(GetResonantCellCount()) + "\n";
        }

        public static string DescriptionStatic() => "向最大生命值最低的敌人发射暗影箭,施加易伤Debuff并造成伤害";

        public override string Name() => NameStatic();
        public static string NameStatic() => "暗影箭";
    }

}