using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FrostZone : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.FrostZone;
        public FrostZoneConfig Config { get; }


        public FrostZone(FrostZoneConfig config)
        {
            RequiredEnergy = config.RequiredEnergy;
            this.Config = config;
        }


        public float GetRadius(int iceCellCount)
        {
            return Config.BaseRadius + iceCellCount * Config.RadiusPerIceCell;
        }

        public float GetDuration(int iceCellCount)
        {
            return Config.BaseDuration + iceCellCount * Config.DurationPerIceCell;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            var enemiesInRange = caster.FindEnemiesInRange(caster.Attributes.AttackRange);
            if (enemiesInRange.Count == 0)
                return false;

            // 以第一个敌人为中心
            Unit targetEnemy = enemiesInRange.First();
            Vector3 center = targetEnemy.transform.position;
            int iceCellCount = caster.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            float radius = GetRadius(iceCellCount);
            float duration = GetDuration(iceCellCount);
            float damage = Config.BaseDamage + iceCellCount * Config.DamagePerIceCell;
            float chilledDuration = Config.BaseChilledDuration + iceCellCount * Config.ChilledDurationPerIceCell;
            int moveSlowPercent = Config.BaseChilledMoveSlowPercent + iceCellCount * Config.ChilledMoveSlowPercentPerIceCell;
            int atkSlowPercent = Config.BaseChilledAtkSlowPercent + iceCellCount * Config.ChilledAtkSlowPercentPerIceCell;
            int energySlowPercent = Config.BaseChilledEnergySlowPercent + iceCellCount * Config.ChilledEnergySlowPercentPerIceCell;

            var prefab = caster.ProjectileConfig.FrostZonePrefab;
            var frostZoneObj = Object.Instantiate(prefab, center, Quaternion.identity);
            var frostZone = frostZoneObj.GetComponent<Units.Projectiles.FrostZone>();
            frostZone.Initialize(
                caster: caster,
                radius: radius,
                duration: duration,
                damage: damage,
                chilledDuration: chilledDuration,
                moveSlowPercent: moveSlowPercent,
                atkSlowPercent: atkSlowPercent,
                energySlowPercent: energySlowPercent
            );
            frostZone.Activate();
            return true;
        }


        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "在目标区域制造一片极寒地带, 对范围内敌人造成冰属性伤害并降低敌方速度.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "霜域";
    }
}