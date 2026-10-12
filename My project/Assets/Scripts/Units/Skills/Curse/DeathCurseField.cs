using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class DeathCurseField : Skill
    {
        public DeathCurseFieldLevelConfig Config { get; }

        private DeathCurseFieldSkillConfig SkillConfig => (DeathCurseFieldSkillConfig)Definition.Config;
        private Units.Projectiles.CurseFieldZone ownedZone;

        public DeathCurseField(DeathCurseFieldLevelConfig config)
        {
            Config = config;
        }

        private struct DeathCurseFieldStats
        {
            public StatValue Radius;
            public StatValue ZoneLifetime;
            public StatValue ApplyInterval;
            public StatValue DebuffDuration;
            public StatValue AttackReductionPercent;
            public StatValue EnergyRegenReductionPercent;
        }

        private DeathCurseFieldStats CalcStats()
        {
            int curseCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Curse, out var count) ? count : 0;

            return new DeathCurseFieldStats
            {
                Radius = new StatValue("领域半径", Config.BaseRadius, curseCellCount * Config.RadiusPerCurseCell),
                ZoneLifetime = new StatValue("领域持续", Config.BaseZoneLifetime, curseCellCount * Config.ZoneLifetimePerCurseCell),
                ApplyInterval = new StatValue("施加间隔", Config.ApplyInterval),
                DebuffDuration = new StatValue("诅咒持续", Config.DebuffDuration),
                AttackReductionPercent = new StatValue("攻击降低(%)", Config.BaseAttackReductionPercent, curseCellCount * Config.AttackReductionPerCurseCell),
                EnergyRegenReductionPercent = new StatValue("能量回复降低(%)", Config.BaseEnergyRegenReductionPercent, curseCellCount * Config.EnergyRegenReductionPerCurseCell)
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
                $"{stats.AttackReductionPercent}\n" +
                $"{stats.EnergyRegenReductionPercent}";
        }

        public static string DescriptionStatic() => "死亡时在当前位置制造诅咒领域，降低敌方攻击力与能量回复。";

        public override string Name() => NameStatic();

        public static string NameStatic() => "死亡诅咒";


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
                energyRegenReductionPercent: stats.EnergyRegenReductionPercent.Final,
                sourceSkill: this,
                curseDefinition: SkillConfig.CurseBuffDefinition
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

            Units.Projectiles.CurseFieldZone zone = ownedZone;
            ownedZone = null;
            zone.Deactivate();
            Object.Destroy(zone.gameObject);
        }
    }
}
