using System;
using System.Collections.Generic;
using Model.Skills;

namespace Units.Skills
{
    public static class SkillFactory
    {
        private interface ISkillCreator
        {
            Skill Create(SkillConfig config, int level);
        }

        private sealed class ConfiguredSkillCreator<TConfig, TLevelConfig> : ISkillCreator
            where TConfig : SkillConfig<TLevelConfig>
            where TLevelConfig : SkillLevelConfig
        {
            private readonly Func<TLevelConfig, Skill> create;

            public ConfiguredSkillCreator(Func<TLevelConfig, Skill> create)
            {
                this.create = create;
            }

            public Skill Create(SkillConfig config, int level)
            {
                return create(((TConfig)config).GetLevelConfig(level));
            }
        }

        private sealed class ParameterlessSkillCreator<TConfig> : ISkillCreator where TConfig : SkillConfig
        {
            private readonly Func<Skill> create;

            public ParameterlessSkillCreator(Func<Skill> create)
            {
                this.create = create;
            }

            public Skill Create(SkillConfig config, int level) => create();
        }

        private static readonly IReadOnlyDictionary<string, ISkillCreator> creators = new Dictionary<string, ISkillCreator>(StringComparer.Ordinal)
        {
            ["skill.energy_absorb"] = new ConfiguredSkillCreator<EnergyAbsorbSkillConfig, EnergyAbsorbLevelConfig>(config => new EnergyAbsorb(config)),
            ["skill.chain_lightning"] = new ConfiguredSkillCreator<ChainLightningSkillConfig, ChainLightningLevelConfig>(config => new ChainLightning(config)),
            ["skill.thunder_strike"] = new ConfiguredSkillCreator<ThunderStrikeSkillConfig, ThunderStrikeLevelConfig>(config => new ThunderStrike(config)),
            ["skill.blazing_field"] = new ConfiguredSkillCreator<BlazingFieldSkillConfig, BlazingFieldLevelConfig>(config => new BlazingField(config)),
            ["skill.fireball"] = new ConfiguredSkillCreator<FireballSkillConfig, FireballLevelConfig>(config => new Fireball(config)),
            ["skill.flame_inject"] = new ConfiguredSkillCreator<FlameInjectSkillConfig, FlameInjectLevelConfig>(config => new FlameInject(config)),
            ["skill.flame_ring"] = new ConfiguredSkillCreator<FlameRingSkillConfig, FlameRingLevelConfig>(config => new FlameRing(config)),
            ["skill.frost_zone"] = new ConfiguredSkillCreator<FrostZoneSkillConfig, FrostZoneLevelConfig>(config => new FrostZone(config)),
            ["skill.ice_breaker"] = new ConfiguredSkillCreator<IceBreakerSkillConfig, IceBreakerLevelConfig>(config => new IceBreaker(config)),
            ["skill.ice_shield"] = new ConfiguredSkillCreator<IceShieldSkillConfig, IceShieldLevelConfig>(config => new IceShield(config)),
            ["skill.icy_cage"] = new ConfiguredSkillCreator<IcyCageSkillConfig, IcyCageLevelConfig>(config => new IcyCage(config)),
            ["skill.snowball"] = new ConfiguredSkillCreator<SnowballSkillConfig, SnowballLevelConfig>(config => new Snowball(config)),
            ["skill.guard_ally"] = new ParameterlessSkillCreator<GuardAllySkillConfig>(() => new GuardAlly()),
            ["skill.life_bomb"] = new ConfiguredSkillCreator<LifeBombSkillConfig, LifeBombLevelConfig>(config => new LifeBomb(config)),
            ["skill.life_echo"] = new ConfiguredSkillCreator<LifeEchoSkillConfig, LifeEchoLevelConfig>(config => new LifeEcho(config)),
            ["skill.life_power"] = new ConfiguredSkillCreator<LifePowerSkillConfig, LifePowerLevelConfig>(config => new LifePower(config)),
            ["skill.life_shield"] = new ConfiguredSkillCreator<LifeShieldSkillConfig, LifeShieldLevelConfig>(config => new LifeShield(config)),
            ["skill.shadow_arrow"] = new ConfiguredSkillCreator<ShadowArrowSkillConfig, ShadowArrowLevelConfig>(config => new ShadowArrow(config)),
            ["skill.shadow_attack"] = new ConfiguredSkillCreator<ShadowAttackSkillConfig, ShadowAttackLevelConfig>(config => new ShadowAttack(config)),
            ["skill.shadow_step"] = new ConfiguredSkillCreator<ShadowStepSkillConfig, ShadowStepLevelConfig>(config => new ShadowStep(config)),
            ["skill.vulnerability_field"] = new ConfiguredSkillCreator<VulnerabilityFieldSkillConfig, VulnerabilityFieldLevelConfig>(config => new VulnerabilityField(config)),
            ["skill.charge"] = new ConfiguredSkillCreator<ChargeSkillConfig, ChargeLevelConfig>(config => new Charge(config)),
            ["skill.attack_boost"] = new ConfiguredSkillCreator<AttackBoostSkillConfig, AttackBoostLevelConfig>(config => new AttackBoost(config)),
            ["skill.wild_wind"] = new ConfiguredSkillCreator<WildWindSkillConfig, WildWindLevelConfig>(config => new WildWind(config)),
            ["skill.wind_knockback"] = new ConfiguredSkillCreator<WindKnockbackSkillConfig, WindKnockbackLevelConfig>(config => new WindKnockback(config)),
            ["skill.wind_shift"] = new ConfiguredSkillCreator<WindShiftSkillConfig, WindShiftLevelConfig>(config => new WindShift(config))
        };

        public static Skill Create(SkillDefinition skillDefinition, int level)
        {
            ISkillCreator creator = creators[skillDefinition.Id];
            Skill skill = creator.Create(skillDefinition.Config, level);
            skill.Definition = skillDefinition;
            return skill;
        }

    }
}
