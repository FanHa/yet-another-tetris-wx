using System.Linq;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class LifeBomb : ActiveSkill
    {
        public LifeBombLevelConfig Config { get; }
        private Vector3 cachedTargetPos; 

        private LifeBombSkillConfig SkillConfig => (LifeBombSkillConfig)Definition.Config;

        public LifeBomb(LifeBombLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        private struct LifeBombStats
        {
            public StatValue HealthCostPercent;
        }

        private LifeBombStats CalcStats()
        {
            return new LifeBombStats
            {
                HealthCostPercent = new StatValue("消耗生命值百分比", Config.HealthCostPercent)
            };
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
            var stats = CalcStats();
            float percent = stats.HealthCostPercent.Final / 100f;
            float healthCost = Owner.Attributes.CurrentHealth * percent;
            healthCost = Mathf.Clamp(healthCost, 1f, Owner.Attributes.CurrentHealth); // 至少消耗1点
            var tempTarget = Object.Instantiate(
                SkillConfig.TemporaryTargetPrefab,
                cachedTargetPos,
                Quaternion.identity
            );
            tempTarget.name = "临时目标 By " + Owner.name + " " + Name();

            // 投射炸弹
            Units.Projectiles.LifeBomb lifeBomb = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );

            lifeBomb.Init(
                caster: Owner.SelfUnit,
                temporaryTarget: tempTarget,
                healthAmount: healthCost, // 伤害为消耗的生命值
                sourceSkill: this
            );
            
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + "\n" +
                $"{stats.HealthCostPercent}";
        }

        public static string DescriptionStatic() => "消耗自身生命值制作炸弹，对目标区域的敌人造成伤害。";


        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命炸弹";
    }
}