using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class FrostZone : ActiveSkill
    {
        public FrostZoneLevelConfig Config { get; }
        private Vector3 targetPosition;
        private readonly HashSet<Units.Projectiles.FrostZone> spawnedZones = new();

        private FrostZoneSkillConfig SkillConfig => (FrostZoneSkillConfig)Definition.Config;


        public FrostZone(FrostZoneLevelConfig config)
        {
            RequiredEnergy = config.RequiredEnergy;
            this.Config = config;
        }

        protected override bool PrepareCastCore()
        {
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var targetEnemy))
                return false;
            targetPosition = targetEnemy.transform.position;
            return true;
        }
        protected override void ExecuteCore()
        {
            var stats = CalcStats();
            spawnedZones.RemoveWhere(zone => zone == null);

            var frostZone = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                targetPosition,
                Quaternion.identity
            );
            spawnedZones.Add(frostZone);
            frostZone.Initialize(
                caster: Owner.SelfUnit,
                radius: stats.Radius.Final,
                duration: stats.Duration.Final,
                damage: stats.Damage.Final,
                chilledDuration: stats.ChilledDuration.Final,
                moveSlowPercent: Mathf.RoundToInt(stats.MoveSlowPercent.Final),
                atkSlowPercent: Mathf.RoundToInt(stats.AtkSlowPercent.Final),
                actionSlowPercent: Mathf.RoundToInt(stats.ActionSlowPercent.Final),
                energySlowPercent: Mathf.RoundToInt(stats.EnergySlowPercent.Final),
                sourceSkill: this
            );
            frostZone.Activate();
        }

        public override void OnRelease()
        {
            foreach (var zone in spawnedZones)
            {
                if (zone != null)
                {
                    Object.Destroy(zone.gameObject);
                }
            }

            spawnedZones.Clear();
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
                $"{stats.ActionSlowPercent}\n" +
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
            public StatValue ActionSlowPercent;
            public StatValue EnergySlowPercent;
        }

        private FrostZoneStats CalcStats()
        {
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new FrostZoneStats
            {
                Radius = new StatValue("范围半径", Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                Duration = new StatValue("持续时间", Config.BaseDuration, resonantCellCount * Config.DurationPerResonantCell),
                Damage = new StatValue("冰属性伤害", Config.BaseDamage, resonantCellCount * Config.DamagePerResonantCell),
                ChilledDuration = new StatValue("减速效果持续时间", Config.BaseChilledDuration, resonantCellCount * Config.ChilledDurationPerResonantCell),
                MoveSlowPercent = new StatValue("移动速度降低(%)", Config.BaseChilledMoveSlowPercent, resonantCellCount * Config.ChilledMoveSlowPercentPerResonantCell),
                AtkSlowPercent = new StatValue("攻击速度降低(%)", Config.BaseChilledAtkSlowPercent, resonantCellCount * Config.ChilledAtkSlowPercentPerResonantCell),
                ActionSlowPercent = new StatValue("动作速率降低(%)", Config.BaseChilledActionSlowPercent, resonantCellCount * Config.ChilledActionSlowPercentPerResonantCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseChilledEnergySlowPercent, resonantCellCount * Config.ChilledEnergySlowPercentPerResonantCell)
            };
        }
    }
}