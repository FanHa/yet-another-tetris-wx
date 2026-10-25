using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Fireball : ActiveSkill
    {
        public FireballLevelConfig Config { get; }
        private Unit targetEnemy;

        private FireballSkillConfig SkillConfig => (FireballSkillConfig)Definition.Config;

        public Fireball(FireballLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
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
                SkillConfig.ProjectilePrefab,
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
                DescriptionStatic() + ":\n" +
                SkillConfig.ProjectilePrefab.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "向攻击范围内一个敌人发射火球，造成灼烧。";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "火球";
    }
}