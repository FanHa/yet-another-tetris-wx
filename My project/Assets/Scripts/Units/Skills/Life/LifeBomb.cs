using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifeBomb : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeBomb;
        public LifeBombConfig Config { get; }

        public LifeBomb(LifeBombConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override void ExecuteCore(Unit caster)
        {
            // todo 需要判断自身血量,如果不够就不能施放技能
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange)
                .OrderBy(enemy => Vector3.Distance(caster.transform.position, enemy.transform.position))
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                return;
            }

            Unit targetEnemy = enemiesInRange.First();



            float percent = Config.HealthCostPercent / 100f;
            float healthCost = caster.Attributes.CurrentHealth * percent;
            healthCost = Mathf.Clamp(healthCost, 1f, caster.Attributes.CurrentHealth); // 至少消耗1点

            // 创建临时目标点
            GameObject tempTarget = Object.Instantiate(
                caster.ProjectileConfig.TemporaryTargetPrefab,
                targetEnemy.transform.position,
                Quaternion.identity
            );

            // 投射炸弹
            GameObject projectileInstance = Object.Instantiate(
                caster.ProjectileConfig.LifeBombPrefab,
                caster.projectileSpawnPoint.position,
                Quaternion.identity
            );
            Units.Projectiles.LifeBomb lifeBomb = projectileInstance.GetComponent<Units.Projectiles.LifeBomb>();

            lifeBomb.Init(
                caster: caster,
                temporaryTarget: tempTarget,
                healthAmount: healthCost, // 伤害为消耗的生命值
                sourceSkill: this
            );
        }

        public override string Description()
        {
            return DescriptionStatic();
        }
         public static string DescriptionStatic() => "消耗自身生命值制作炸弹，对目标区域的敌人造成范围伤害。";


        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命炸弹";
    }
}