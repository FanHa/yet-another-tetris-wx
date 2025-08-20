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

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            // 找到攻击范围内的敌人
            var targetEnemy = Owner.UnitManager.FindClosestEnemyInRange(Owner, Owner.Attributes.AttackRange.finalValue);
            if (targetEnemy == null)
                return false;
            targetPosition = targetEnemy.transform.position;
            return true;
        }
        protected override bool ExecuteCore()
        {
            var stats = CalcStats();

            var prefab = Owner.ProjectileConfig.FrostZonePrefab;
            var frostZoneObj = Object.Instantiate(prefab, targetPosition, Quaternion.identity);
            var frostZone = frostZoneObj.GetComponent<Units.Projectiles.FrostZone>();
            frostZone.Initialize(
                caster: Owner,
                radius: stats.Radius.Final,
                duration: stats.Duration.Final,
                damage: stats.Damage.Final,
                chilledDuration: stats.ChilledDuration.Final,
                moveSlowPercent: Mathf.RoundToInt(stats.MoveSlowPercent.Final),
                atkSlowPercent: Mathf.RoundToInt(stats.AtkSlowPercent.Final),
                energySlowPercent: Mathf.RoundToInt(stats.EnergySlowPercent.Final),
                sourceSkill: this
            );
            frostZone.Activate();
            return true;
        }


        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Duration}\n" +
                $"{stats.Radius}\n" +
                $"{stats.Damage}\n" +
                $"{stats.MoveSlowPercent}\n" +
                $"{stats.AtkSlowPercent}\n" +
                $"{stats.EnergySlowPercent}\n" +
                $"{stats.ChilledDuration}";
        }

        public static string DescriptionStatic() => "在目标区域制造一片极寒地带, 对范围内敌人造成冰属性伤害并降低敌方速度.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "霜域";

        private struct FrostZoneStats
        {
            public StatValue Radius;
            public StatValue Duration;
            public StatValue Damage;
            public StatValue ChilledDuration;
            public StatValue MoveSlowPercent;
            public StatValue AtkSlowPercent;
            public StatValue EnergySlowPercent;
        }

        private FrostZoneStats CalcStats()
        {
            int iceCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            return new FrostZoneStats
            {
                Radius = new StatValue("范围半径", Config.BaseRadius, iceCellCount * Config.RadiusPerIceCell),
                Duration = new StatValue("持续时间", Config.BaseDuration, iceCellCount * Config.DurationPerIceCell),
                Damage = new StatValue("冰属性伤害", Config.BaseDamage, iceCellCount * Config.DamagePerIceCell),
                ChilledDuration = new StatValue("减速效果持续时间", Config.BaseChilledDuration, iceCellCount * Config.ChilledDurationPerIceCell),
                MoveSlowPercent = new StatValue("移动速度降低(%)", Config.BaseChilledMoveSlowPercent, iceCellCount * Config.ChilledMoveSlowPercentPerIceCell),
                AtkSlowPercent = new StatValue("攻击速度降低(%)", Config.BaseChilledAtkSlowPercent, iceCellCount * Config.ChilledAtkSlowPercentPerIceCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseChilledEnergySlowPercent, iceCellCount * Config.ChilledEnergySlowPercentPerIceCell)
            };
        }
    }
}