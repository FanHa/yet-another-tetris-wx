using System.Linq;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Snowball : ActiveSkill
    {
        public SnowballLevelConfig Config { get; }
        private Unit cachedTarget;

        private SnowballSkillConfig SkillConfig => (SnowballSkillConfig)Definition.Config;

        public Snowball(SnowballLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Context.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            cachedTarget = found;

            // 可以发射雪球
            return true;
        }

        protected override void ExecuteCore()
        {
            // 实例化雪球投射物
            Units.Projectiles.Snowball snowBall = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Context.ProjectileSpawnPoint.position,
                Quaternion.identity
            );
            snowBall.Init(
                Context.Unit,
                cachedTarget,
                this,
                Context.GetResonanceCount(Definition.Resonance)
            );
            snowBall.Activate();
            // 清空目标
            cachedTarget = null;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + "\n" +
                SkillConfig.ProjectilePrefab.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }
        public static string DescriptionStatic() => "向攻击范围内一个敌人发射雪球,施加减速Debuff.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "雪球";
        
    }
}