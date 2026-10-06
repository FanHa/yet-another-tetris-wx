using System.Linq;
using Model.Tetri;
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

        private struct FireballStats
        {
            public StatValue BurnDps;
            public StatValue BurnDuration;
        }

        private FireballStats CalcStats()
        {
            int fireCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            return new FireballStats
            {
                BurnDps = new StatValue("灼烧每秒伤害", Config.DotBaseDamage, fireCellCount * Config.DotAddtionPerFireCell),
                BurnDuration = new StatValue("灼烧持续时间", Config.DotDuration)
            };
        }
        
        protected override bool PrepareCastCore()
        {
            targetEnemy = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            targetEnemy = found;

            return true;
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            Units.Projectiles.Fireball fireball = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );

            fireball.Init(
                caster: Owner.SelfUnit,
                target: targetEnemy,
                burnDps: stats.BurnDps.Final,
                burnDuration: stats.BurnDuration.Final,
                sourceSkill: this
            );
            fireball.Activate();
            targetEnemy = null; // 用完清空
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.BurnDps}\n" +
                $"{stats.BurnDuration}";
        }

        public static string DescriptionStatic() => "向攻击范围内一个敌人发射火球，造成灼烧。";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "火球";
    }
}