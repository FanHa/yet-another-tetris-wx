using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifeBomb : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeBomb;
        public LifeBombConfig Config { get; }
        private GameObject cachedTempTarget;

        public LifeBomb(LifeBombConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;
            var targetEnemy = Owner.UnitManager.FindRandomEnemyInRange(Owner, Owner.Attributes.AttackRange);
            if (targetEnemy == null)
                return false;
            cachedTempTarget = Object.Instantiate(
                Owner.ProjectileConfig.TemporaryTargetPrefab,
                targetEnemy.transform.position,
                Quaternion.identity
            );
            return true;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            float percent = Config.HealthCostPercent / 100f;
            float healthCost = caster.Attributes.CurrentHealth * percent;
            healthCost = Mathf.Clamp(healthCost, 1f, caster.Attributes.CurrentHealth); // 至少消耗1点


            // 投射炸弹
            GameObject projectileInstance = Object.Instantiate(
                caster.ProjectileConfig.LifeBombPrefab,
                caster.projectileSpawnPoint.position,
                Quaternion.identity
            );
            Units.Projectiles.LifeBomb lifeBomb = projectileInstance.GetComponent<Units.Projectiles.LifeBomb>();

            lifeBomb.Init(
                caster: caster,
                temporaryTarget: cachedTempTarget,
                healthAmount: healthCost, // 伤害为消耗的生命值
                sourceSkill: this
            );
            cachedTempTarget = null; // 用完清空
            return true;
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