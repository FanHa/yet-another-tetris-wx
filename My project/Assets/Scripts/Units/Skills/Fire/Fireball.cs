using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Fireball : ActiveSkill
    {
        private readonly FireballSkillDefinition skillDefinition;
        private Unit targetEnemy;

        public Fireball(FireballSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            targetEnemy = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Context.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            targetEnemy = found;

            return true;
        }

        protected override void ExecuteCore()
        {
            Units.Projectiles.Fireball fireball = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Context.ProjectileSpawnPoint.position,
                Quaternion.identity
            );

            fireball.Init(
                caster: Context.Unit,
                target: targetEnemy,
                sourceSkill: this,
                resonantCellCount: Context.GetResonanceCount(Definition.Resonance)
            );
            fireball.Activate();
            targetEnemy = null; // 用完清空
        }

        public override string DescribeEffect()
        {
            return
                Definition.Description + ":\n" +
                skillDefinition.ProjectilePrefab.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public override string Name() => Definition.DisplayName;
    }
}