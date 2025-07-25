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
        private Vector3 targetPosition;


        public FrostZone(FrostZoneConfig config)
        {
            RequiredEnergy = config.RequiredEnergy;
            this.Config = config;
        }


        private float GetRadius(int iceCellCount)
        {
            return Config.BaseRadius + iceCellCount * Config.RadiusPerIceCell;
        }

        private float GetDuration(int iceCellCount)
        {
            return Config.BaseDuration + iceCellCount * Config.DurationPerIceCell;
        }
        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            // 找到攻击范围内的敌人
            var targetEnemy = Owner.UnitManager.FindClosestEnemyInRange(Owner, Owner.Attributes.AttackRange);
            if (targetEnemy == null)
                return false;
            targetPosition = targetEnemy.transform.position;
            return true;
        }
        protected override bool ExecuteCore()
        {
            int iceCellCount = Owner.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            float radius = GetRadius(iceCellCount);
            float duration = GetDuration(iceCellCount);
            float damage = Config.BaseDamage + iceCellCount * Config.DamagePerIceCell;
            float chilledDuration = Config.BaseChilledDuration + iceCellCount * Config.ChilledDurationPerIceCell;
            int moveSlowPercent = Config.BaseChilledMoveSlowPercent + iceCellCount * Config.ChilledMoveSlowPercentPerIceCell;
            int atkSlowPercent = Config.BaseChilledAtkSlowPercent + iceCellCount * Config.ChilledAtkSlowPercentPerIceCell;
            int energySlowPercent = Config.BaseChilledEnergySlowPercent + iceCellCount * Config.ChilledEnergySlowPercentPerIceCell;

            var prefab = Owner.ProjectileConfig.FrostZonePrefab;
            var frostZoneObj = Object.Instantiate(prefab, targetPosition, Quaternion.identity);
            var frostZone = frostZoneObj.GetComponent<Units.Projectiles.FrostZone>();
            frostZone.Initialize(
                caster: Owner,
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