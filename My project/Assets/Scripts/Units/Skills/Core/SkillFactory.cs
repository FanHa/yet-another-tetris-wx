using System;
using System.Collections.Generic;
using Model.Skills;

namespace Units.Skills
{
    public static class SkillFactory
    {
        private interface ISkillCreator
        {
            Type DefinitionType { get; }
            Skill Create(SkillDefinition definition, int level);
        }

        private sealed class DefinitionSkillCreator<TDefinition> : ISkillCreator where TDefinition : SkillDefinition
        {
            private readonly Func<TDefinition, Skill> create;

            public DefinitionSkillCreator(Func<TDefinition, Skill> create)
            {
                this.create = create;
            }

            public Type DefinitionType => typeof(TDefinition);
            public Skill Create(SkillDefinition definition, int level) => create((TDefinition)definition);
        }

        private static readonly IReadOnlyDictionary<string, ISkillCreator> creators = new Dictionary<string, ISkillCreator>(StringComparer.Ordinal)
        {
            ["skill.energy_absorb"] = new DefinitionSkillCreator<EnergyAbsorbSkillDefinition>(definition => new EnergyAbsorb()),
            ["skill.chain_lightning"] = new DefinitionSkillCreator<ChainLightningSkillDefinition>(definition => new ChainLightning(definition)),
            ["skill.thunder_strike"] = new DefinitionSkillCreator<ThunderStrikeSkillDefinition>(definition => new ThunderStrike(definition)),
            ["skill.electric_field"] = new DefinitionSkillCreator<ElectricFieldSkillDefinition>(definition => new ElectricField(definition)),
            ["skill.blazing_field"] = new DefinitionSkillCreator<BlazingFieldSkillDefinition>(definition => new BlazingField(definition)),
            ["skill.fireball"] = new DefinitionSkillCreator<FireballSkillDefinition>(definition => new Fireball(definition)),
            ["skill.flame_inject"] = new DefinitionSkillCreator<FlameInjectSkillDefinition>(definition => new FlameInject()),
            ["skill.flame_ring"] = new DefinitionSkillCreator<FlameRingSkillDefinition>(definition => new FlameRing(definition)),
            ["skill.frost_zone"] = new DefinitionSkillCreator<FrostZoneSkillDefinition>(definition => new FrostZone(definition)),
            ["skill.ice_breaker"] = new DefinitionSkillCreator<IceBreakerSkillDefinition>(definition => new IceBreaker()),
            ["skill.ice_shield"] = new DefinitionSkillCreator<IceShieldSkillDefinition>(definition => new IceShield()),
            ["skill.icy_cage"] = new DefinitionSkillCreator<IcyCageSkillDefinition>(definition => new IcyCage(definition)),
            ["skill.snowball"] = new DefinitionSkillCreator<SnowballSkillDefinition>(definition => new Snowball(definition)),
            ["skill.guard_ally"] = new DefinitionSkillCreator<SkillDefinition>(definition => new GuardAlly()),
            ["skill.life_bomb"] = new DefinitionSkillCreator<LifeBombSkillDefinition>(definition => new LifeBomb(definition)),
            ["skill.life_echo"] = new DefinitionSkillCreator<LifeEchoSkillDefinition>(definition => new LifeEcho()),
            ["skill.life_power"] = new DefinitionSkillCreator<LifePowerSkillDefinition>(definition => new LifePower(definition)),
            ["skill.life_shield"] = new DefinitionSkillCreator<LifeShieldSkillDefinition>(definition => new LifeShield(definition)),
            ["skill.life_fortitude"] = new DefinitionSkillCreator<LifeFortitudeSkillDefinition>(definition => new LifeFortitude()),
            ["skill.shadow_arrow"] = new DefinitionSkillCreator<ShadowArrowSkillDefinition>(definition => new ShadowArrow(definition)),
            ["skill.shadow_attack"] = new DefinitionSkillCreator<ShadowAttackSkillDefinition>(definition => new ShadowAttack(definition)),
            ["skill.shadow_step"] = new DefinitionSkillCreator<ShadowStepSkillDefinition>(definition => new ShadowStep(definition)),
            ["skill.vulnerability_field"] = new DefinitionSkillCreator<VulnerabilityFieldSkillDefinition>(definition => new VulnerabilityField(definition)),
            ["skill.power_curse"] = new DefinitionSkillCreator<PowerCurseSkillDefinition>(definition => new PowerCurse()),
            ["skill.energy_curse"] = new DefinitionSkillCreator<EnergyCurseSkillDefinition>(definition => new EnergyCurse()),
            ["skill.charge"] = new DefinitionSkillCreator<ChargeSkillDefinition>(definition => new Charge(definition)),
            ["skill.attack_boost"] = new DefinitionSkillCreator<AttackBoostSkillDefinition>(definition => new AttackBoost(definition)),
            ["skill.wild_wind"] = new DefinitionSkillCreator<WildWindSkillDefinition>(definition => new WildWind(definition)),
            ["skill.wind_knockback"] = new DefinitionSkillCreator<WindKnockbackSkillDefinition>(definition => new WindKnockback(definition)),
            ["skill.wind_shift"] = new DefinitionSkillCreator<WindShiftSkillDefinition>(definition => new WindShift(definition))
        };

        public static bool TryGetRequiredDefinitionType(string skillId, out Type definitionType)
        {
            if (creators.TryGetValue(skillId, out ISkillCreator creator))
            {
                definitionType = creator.DefinitionType;
                return true;
            }

            definitionType = null;
            return false;
        }

        public static IEnumerable<string> GetRegisteredSkillIds() => creators.Keys;

        public static Skill Create(SkillDefinition skillDefinition, int level)
        {
            ISkillCreator creator = creators[skillDefinition.Id];
            Skill skill = creator.Create(skillDefinition, level);
            skill.Definition = skillDefinition;
            return skill;
        }

    }
}
