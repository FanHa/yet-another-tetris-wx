using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Snowball : ActiveSkill
    {
        private readonly SnowballSkillDefinition skillDefinition;

        public Snowball(SnowballSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override void ExecuteCore()
        {
            Unit targetEnemy = UnitManager.FindClosestEnemy(Owner);
            if (targetEnemy == null)
            {
                return;
            }

            // 实例化雪球投射物
            Units.Projectiles.Snowball snowBall = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );
            snowBall.Init(
                Owner,
                targetEnemy,
                this,
                Owner.GetResonanceCount(Definition.Resonance)
            );
            snowBall.Activate();
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