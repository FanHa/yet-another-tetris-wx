using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class Fireball : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.Fireball;
        public FireballConfig Config { get; }

        public Fireball(FireballConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            var enemiesInRange = caster.FindEnemiesInRange(caster.Attributes.AttackRange)
                .OrderBy(enemy => Vector3.Distance(caster.transform.position, enemy.transform.position))
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                return false;
            }

            Unit targetEnemy = enemiesInRange.First();
            GameObject projectileInstance = Object.Instantiate(caster.ProjectileConfig.FireballPrefab, caster.projectileSpawnPoint.position, Quaternion.identity);
            Units.Projectiles.Fireball fireball = projectileInstance.GetComponent<Units.Projectiles.Fireball>();
            int fireCellCount = caster.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            
            float burnDps = Config.DotBaseDamage + fireCellCount * Config.DotAddtionPerFireCell;
            float burnDuration = Config.DotDuration;

            fireball.Init(
                caster: caster,
                target: targetEnemy,
                burnDps: burnDps,
                burnDuration: burnDuration,
                sourceSkill: this
            );
            fireball.Activate();

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