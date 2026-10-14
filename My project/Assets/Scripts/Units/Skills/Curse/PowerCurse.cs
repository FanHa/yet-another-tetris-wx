using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class PowerCurse : Skill
    {
        public PowerCurseLevelConfig Config { get; }

        private PowerCurseSkillConfig SkillConfig => (PowerCurseSkillConfig)Definition.Config;
        private Units.Projectiles.PowerCurseZone ownedZone;

        public PowerCurse(PowerCurseLevelConfig config)
        {
            Config = config;
        }

        private struct PowerCurseStats
        {
            public StatValue Radius;
            public StatValue ZoneLifetime;
            public StatValue ApplyInterval;
            public StatValue DebuffDuration;
            public StatValue AttackReductionPercent;
        }

        private PowerCurseStats CalcStats()
        {
            int deathCurseCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.DeathCurse, out var count) ? count : 0;

            return new PowerCurseStats
            {
                Radius = new StatValue("领域半径", Config.BaseRadius, deathCurseCellCount * Config.RadiusPerDeathCurseCell),
                ZoneLifetime = new StatValue("领域持续", Config.BaseZoneLifetime, deathCurseCellCount * Config.ZoneLifetimePerDeathCurseCell),
                ApplyInterval = new StatValue("施加间隔", Config.ApplyInterval),
                DebuffDuration = new StatValue("诅咒持续", Config.DebuffDuration),
                AttackReductionPercent = new StatValue("攻击降低(%)", Config.BaseAttackReductionPercent, deathCurseCellCount * Config.AttackReductionPerDeathCurseCell)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.ZoneLifetime}\n" +
                $"{stats.ApplyInterval}\n" +
                $"{stats.DebuffDuration}\n" +
                $"{stats.AttackReductionPercent}";
        }

        public static string DescriptionStatic() => "死亡时在当前位置生成诅咒区域，降低区域内敌人的攻击力。";

        public override string Name() => NameStatic();

        public static string NameStatic() => "力量诅咒";


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
                applyInterval: stats.ApplyInterval.Final,
                debuffDuration: stats.DebuffDuration.Final,
                attackReductionPercent: stats.AttackReductionPercent.Final,
                sourceSkill: this,
                powerCurseBuffDefinition: SkillConfig.PowerCurseBuffDefinition
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

            Units.Projectiles.PowerCurseZone zone = ownedZone;
            ownedZone = null;
            zone.Deactivate();
            Object.Destroy(zone.gameObject);
        }
    }
}
