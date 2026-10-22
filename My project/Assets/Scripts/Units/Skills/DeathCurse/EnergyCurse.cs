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
        }

        private EnergyCurseStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();

            return new EnergyCurseStats
            {
                Radius = new StatValue("领域半径", Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                ZoneLifetime = new StatValue("领域持续", Config.BaseZoneLifetime, resonantCellCount * Config.ZoneLifetimePerResonantCell)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.ZoneLifetime}\n" +
                SkillConfig.EnergyCurseBuffDefinition.DescribeEffect(GetResonantCellCount());
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
                resonantCellCount: GetResonantCellCount(),
                sourceSkill: this,
                energyCurseBuffDefinition: SkillConfig.EnergyCurseBuffDefinition
            );
            ownedZone.Activate();
        }

        public override void OnRelease()
        {
            ClearOwnedZone();
        }

        private int GetResonantCellCount() => Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;

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
