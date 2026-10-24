using System.Collections.Generic;
using Model;
using Model.Skills;
using Model.Resonance;
using UnityEngine;

namespace Units.Skills
{
    public sealed class ElectricField : ActiveSkill
    {
        public ElectricFieldLevelConfig Config { get; }

        private readonly HashSet<Units.Projectiles.ElectricFieldZone> spawnedZones = new();
        private Vector3 targetPosition;
        private ElectricFieldSkillConfig SkillConfig => (ElectricFieldSkillConfig)Definition.Config;

        public ElectricField(ElectricFieldLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            if (!Context.TryGetClosestEnemyInAttackRange(out Unit target))
            {
                return false;
            }

            targetPosition = target.transform.position;
            return true;
        }

        protected override void ExecuteCore()
        {
            ElectricFieldStats stats = CalculateStats();
            spawnedZones.RemoveWhere(zone => zone == null);

            Units.Projectiles.ElectricFieldZone zone = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                targetPosition,
                Quaternion.identity);
            spawnedZones.Add(zone);
            zone.Initialize(
                caster: Context.Unit,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                hitProjectilePrefab: SkillConfig.HitProjectilePrefab,
                damage: stats.Damage.FinalValue,
                energyDisruptionAmount: stats.EnergyDisruptionAmount.FinalValue,
                sourceSkill: this);
            zone.Activate();
        }

        public override void OnRelease()
        {
            foreach (Units.Projectiles.ElectricFieldZone zone in spawnedZones)
            {
                if (zone != null)
                {
                    Object.Destroy(zone.gameObject);
                }
            }

            spawnedZones.Clear();
        }

        public override string Name() => "电场";

        public override string Description()
        {
            ElectricFieldStats stats = CalculateStats();
            return "在敌人所在区域制造电场，每隔一段时间随机电击一名区域内敌人，并削减其随机技能蓄能：\n"
                   + $"{stats.Radius}\n"
                   + $"{stats.Duration}\n"
                   + $"触发间隔: <final>{SkillConfig.ProjectilePrefab.ApplyInterval}</final>\n"
                   + $"{stats.Damage}\n"
                   + $"{stats.EnergyDisruptionAmount}";
        }

        private ElectricFieldStats CalculateStats()
        {
            int resonantCellCount = Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;

            return new ElectricFieldStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.AreaRadius, Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                Duration = new StatBreakdownDisplay(StatLabel.Duration, Config.BaseDuration, resonantCellCount * Config.DurationPerResonantCell),
                Damage = new StatBreakdownDisplay(StatLabel.ElectricDamage, Config.BaseDamage, resonantCellCount * Config.DamagePerResonantCell),
                EnergyDisruptionAmount = new StatBreakdownDisplay(
                    StatLabel.SkillEnergyReduction,
                    Config.BaseEnergyDisruptionAmount,
                    resonantCellCount * Config.EnergyDisruptionAmountPerResonantCell)
            };
        }

        private struct ElectricFieldStats
        {
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay Duration;
            public StatBreakdownDisplay Damage;
            public StatBreakdownDisplay EnergyDisruptionAmount;
        }
    }
}