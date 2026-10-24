using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
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
            if (!Context.TryGetClosestEnemyInAttackRange(out var targetEnemy))
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
                caster: Context.Unit,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                damage: stats.Damage.FinalValue,
                sourceSkill: this,
                resonantCellCount: GetResonantCellCount()
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
                SkillConfig.ProjectilePrefab.DescribeEffect(GetResonantCellCount());
        }

        public static string DescriptionStatic() => "在目标区域制造一片极寒地带, 对范围内敌人造成冰属性伤害并降低敌方速度.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "霜域";

        private int GetResonantCellCount()
        {
            return Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;
        }

        private struct FrostZoneStats
        {
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay Duration;
            public StatBreakdownDisplay Damage;
        }

        private FrostZoneStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            return new FrostZoneStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.AreaRadius, Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                Duration = new StatBreakdownDisplay(StatLabel.Duration, Config.BaseDuration, resonantCellCount * Config.DurationPerResonantCell),
                Damage = new StatBreakdownDisplay(StatLabel.SkillDamage, Config.BaseDamage, resonantCellCount * Config.DamagePerResonantCell)
            };
        }
    }
}