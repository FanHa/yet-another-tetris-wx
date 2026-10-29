using System.Linq;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Snowball : ActiveSkill
    {
        private readonly SnowballSkillDefinition skillDefinition;
        private Unit cachedTarget;

        public Snowball(SnowballSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            cachedTarget = found;

            // 可以发射雪球
            return true;
        }

        protected override void ExecuteCore()
        {
            // 实例化雪球投射物
            Units.Projectiles.Snowball snowBall = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );
            snowBall.Init(
                Owner,
                cachedTarget,
                this,
                Owner.GetResonanceCount(Definition.Resonance)
            );
            snowBall.Activate();
            // 清空目标
            cachedTarget = null;
        }

        public override string DescribeEffect()
        {
            return
                Definition.Description + "\n" +
                skillDefinition.ProjectilePrefab.DescribeEffect(Owner.GetResonanceCount(Definition.Resonance));
        }

        public override string Name() => Definition.DisplayName;
        
    }
}