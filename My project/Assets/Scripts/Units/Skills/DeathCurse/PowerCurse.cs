using Model;
using Presentation;
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
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay ZoneLifetime;
        }

        private PowerCurseStats CalcStats()
        {
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);

            return new PowerCurseStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.DomainRadius, Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                ZoneLifetime = new StatBreakdownDisplay(StatLabel.DomainDuration, Config.BaseZoneLifetime, resonantCellCount * Config.ZoneLifetimePerResonantCell)
            };
        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.ZoneLifetime}\n" +
                SkillConfig.PowerCurseBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "死亡时在当前位置生成诅咒区域，降低区域内敌人的攻击力。";

        public override string Name() => NameStatic();

        public static string NameStatic() => "力量诅咒";


        public override void OnUnitDeath()
        {
            var stats = CalcStats();

            ownedZone = Object.Instantiate(
                SkillConfig.ZonePrefab,
                Context.Transform.position,
                Quaternion.identity
            );

            ownedZone.Initialize(
                caster: Context.Unit,
                radius: stats.Radius.FinalValue,
                duration: stats.ZoneLifetime.FinalValue,
                resonantCellCount: Context.GetResonanceCount(Definition.Resonance),
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
