using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class Fireball : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.Fireball;
        public FireballConfig Config { get; }
        private Unit targetEnemy;

        public Fireball(FireballConfig config)
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
        
        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            // 找到攻击范围内的敌人
            targetEnemy = Owner.UnitManager.FindClosestEnemyInRange(Owner, Owner.Attributes.AttackRange);
            if (targetEnemy == null)
                return false;

            return true;
        }

        protected override bool ExecuteCore()
        {
            // todo targetEnemy 是否依然活跃
            var stats = CalcStats();

            GameObject projectileInstance = Object.Instantiate(Owner.ProjectileConfig.FireballPrefab, Owner.projectileSpawnPoint.position, Quaternion.identity);
            Units.Projectiles.Fireball fireball = projectileInstance.GetComponent<Units.Projectiles.Fireball>();

            fireball.Init(
                caster: Owner,
                target: targetEnemy,
                burnDps: stats.BurnDps.Final,
                burnDuration: stats.BurnDuration.Final,
                sourceSkill: this
            );
            fireball.Activate();
            targetEnemy = null; // 用完清空
            return true;
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