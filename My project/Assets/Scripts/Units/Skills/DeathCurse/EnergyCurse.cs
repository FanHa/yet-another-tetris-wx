using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class EnergyCurse : Skill
    {
        public EnergyCurseLevelConfig Config { get; }

        private EnergyCurseSkillConfig SkillConfig => (EnergyCurseSkillConfig)Definition.Config;
        private Units.Projectiles.EnergyCurseZone ownedZone;

        public EnergyCurse(EnergyCurseLevelConfig config)
        {
            Config = config;
        }

        private struct EnergyCurseStats
        {
            public StatValue Radius;
            public StatValue ZoneLifetime;
            public StatValue DebuffDuration;
            public StatValue EnergyRegenReductionPercent;
        }

        private EnergyCurseStats CalcStats()
        {
            int deathCurseCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.DeathCurse, out var count) ? count : 0;
            float energyRegenReductionBonus = Mathf.Min(
                deathCurseCellCount * Config.EnergyRegenReductionPerDeathCurseCell,
                Config.MaxEnergyRegenReductionPercent - Config.BaseEnergyRegenReductionPercent);

            return new EnergyCurseStats
            {
                Radius = new StatValue("领域半径", Config.BaseRadius, deathCurseCellCount * Config.RadiusPerDeathCurseCell),
                ZoneLifetime = new StatValue("领域持续", Config.BaseZoneLifetime, deathCurseCellCount * Config.ZoneLifetimePerDeathCurseCell),
                DebuffDuration = new StatValue("诅咒持续", Config.DebuffDuration),
                EnergyRegenReductionPercent = new StatValue("能量回复降低(%)", Config.BaseEnergyRegenReductionPercent, energyRegenReductionBonus)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.ZoneLifetime}\n" +
                $"{stats.DebuffDuration}\n" +
                $"{stats.EnergyRegenReductionPercent}";
        }

        public static string DescriptionStatic() => "死亡时在当前位置生成诅咒区域，降低区域内敌人的能量回复速度。";

        public override string Name() => NameStatic();

        public static string NameStatic() => "能量诅咒";

        public override void OnUnitDeath()
        {
            var stats = CalcStats();

            ownedZone = Object.Instantiate(
                SkillConfig.ZonePrefab,
                Owner.transform.position,
                Quaternion.identity
            );

            ownedZone.Initialize(
                caster: Owner.SelfUnit,
                radius: stats.Radius.Final,
                duration: stats.ZoneLifetime.Final,
                debuffDuration: stats.DebuffDuration.Final,
                energyRegenReductionPercent: stats.EnergyRegenReductionPercent.Final,
                sourceSkill: this,
                energyCurseBuffDefinition: SkillConfig.EnergyCurseBuffDefinition
            );
            ownedZone.Activate();
        }

        public override void OnRelease()
        {
            ClearOwnedZone();
        }

        private void ClearOwnedZone()
        {
            if (ownedZone == null)
                return;

            Units.Projectiles.EnergyCurseZone zone = ownedZone;
            ownedZone = null;
            zone.Deactivate();
            Object.Destroy(zone.gameObject);
        }
    }
}
