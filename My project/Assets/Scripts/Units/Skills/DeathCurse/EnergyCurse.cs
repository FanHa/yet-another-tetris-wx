using Model;
using Presentation;
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
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay ZoneLifetime;
        }

        private EnergyCurseStats CalcStats()
        {
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);

            return new EnergyCurseStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.DomainRadius, Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                ZoneLifetime = new StatBreakdownDisplay(StatLabel.DomainDuration, Config.BaseZoneLifetime, resonantCellCount * Config.ZoneLifetimePerResonantCell)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.ZoneLifetime}\n" +
                SkillConfig.EnergyCurseBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "死亡时在当前位置生成诅咒区域，降低区域内敌人的能量回复速度。";

        public override string Name() => NameStatic();

        public static string NameStatic() => "能量诅咒";

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
