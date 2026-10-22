using Model.Resonance;
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
        }

        private PowerCurseStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();

            return new PowerCurseStats
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
                SkillConfig.PowerCurseBuffDefinition.DescribeEffect(GetResonantCellCount());
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
                resonantCellCount: GetResonantCellCount(),
                sourceSkill: this,
                powerCurseBuffDefinition: SkillConfig.PowerCurseBuffDefinition
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

            Units.Projectiles.PowerCurseZone zone = ownedZone;
            ownedZone = null;
            zone.Deactivate();
            Object.Destroy(zone.gameObject);
        }
    }
}
