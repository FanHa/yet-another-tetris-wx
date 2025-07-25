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
            GameObject projectileInstance = Object.Instantiate(Owner.ProjectileConfig.FireballPrefab, Owner.projectileSpawnPoint.position, Quaternion.identity);
            Units.Projectiles.Fireball fireball = projectileInstance.GetComponent<Units.Projectiles.Fireball>();
            int fireCellCount = Owner.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;

            float burnDps = Config.DotBaseDamage + fireCellCount * Config.DotAddtionPerFireCell;
            float burnDuration = Config.DotDuration;

            fireball.Init(
                caster: Owner,
                target: targetEnemy,
                burnDps: burnDps,
                burnDuration: burnDuration,
                sourceSkill: this
            );
            fireball.Activate();
            targetEnemy = null; // 用完清空
            return true;
        }
        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "向攻击范围内一个敌人发射火球，造成灼烧。";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "火球";
    }
}