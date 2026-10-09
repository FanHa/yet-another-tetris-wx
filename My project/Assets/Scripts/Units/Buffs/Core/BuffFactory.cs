using System;
using System.Collections.Generic;
using Model.Buffs;

namespace Units.Buffs
{
    public static class BuffFactory
    {
        private interface IFactoryEntry
        {
            Type ArgsType { get; }
            Buff Create(BuffDefinition definition, object args);
        }

        private sealed class FactoryEntry<TArgs> : IFactoryEntry where TArgs : struct, IBuffCreationArgs
        {
            private readonly Func<BuffDefinition, TArgs, Buff> create;

            public FactoryEntry(Func<BuffDefinition, TArgs, Buff> create)
            {
                this.create = create;
            }

            public Type ArgsType => typeof(TArgs);

            public Buff Create(BuffDefinition definition, object args)
            {
                return create(definition, (TArgs)args);
            }
        }

        private static readonly Dictionary<string, IFactoryEntry> factories = new(StringComparer.Ordinal)
        {
            ["buff.flame_inject"] = new FactoryEntry<FlameInjectBuffArgs>((definition, args) =>
                new FlameInject(definition, args)),
            ["buff.burn"] = new FactoryEntry<BurnBuffArgs>((definition, args) =>
                new Burn(definition, args)),
            ["buff.attack_boost"] = new FactoryEntry<AttackBoostBuffArgs>((definition, args) =>
                new AttackBoostBuff(definition, args)),
            ["buff.chilled"] = new FactoryEntry<ChilledBuffArgs>((definition, args) =>
                new Chilled(definition, args)),
            ["buff.energy_absorb"] = new FactoryEntry<EnergyAbsorbBuffArgs>((definition, args) =>
                new EnergyAbsorb(definition, args)),
            ["buff.freeze"] = new FactoryEntry<FreezeBuffArgs>((definition, args) =>
                new Freeze(definition, args)),
            ["buff.ice_breaker"] = new FactoryEntry<IceBreakerBuffArgs>((definition, args) =>
                new IceBreaker(definition, args)),
            ["buff.ice_shield"] = new FactoryEntry<IceShieldBuffArgs>((definition, args) =>
                new IceShield(definition, args)),
            ["buff.life_echo"] = new FactoryEntry<LifeEchoBuffArgs>((definition, args) =>
                new LifeEchoBuff(definition, args)),
            ["buff.life_power"] = new FactoryEntry<LifePowerBuffArgs>((definition, args) =>
                new LifePowerBuff(definition, args)),
            ["buff.life_shield"] = new FactoryEntry<LifeShieldBuffArgs>((definition, args) =>
                new LifeShieldBuff(definition, args)),
            ["buff.shadow_attack"] = new FactoryEntry<ShadowAttackBuffArgs>((definition, args) =>
                new ShadowAttackBuff(definition, args)),
            ["buff.thunder_strike"] = new FactoryEntry<ThunderStrikeBuffArgs>((definition, args) =>
                new ThunderStrikeBuff(definition, args)),
            ["buff.vulnerability"] = new FactoryEntry<VulnerabilityBuffArgs>((definition, args) =>
                new Vulnerability(definition, args)),
            ["buff.wild_wind_debuff"] = new FactoryEntry<WildWindDebuffBuffArgs>((definition, args) =>
                new WildWindDebuff(definition, args)),
            ["buff.wind_knockback"] = new FactoryEntry<WindKnockbackBuffArgs>((definition, args) =>
                new WindKnockbackBuff(definition, args)),
            ["buff.wind_shift"] = new FactoryEntry<WindShiftBuffArgs>((definition, args) =>
                new WindShiftBuff(definition, args))
        };

        public static Buff Create<TArgs>(BuffDefinition definition, TArgs args) where TArgs : struct, IBuffCreationArgs
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (string.IsNullOrWhiteSpace(definition.Id))
            {
                throw new InvalidOperationException($"Buff definition '{definition.name}' has no ID.");
            }

            if (!factories.TryGetValue(definition.Id, out IFactoryEntry entry))
            {
                throw new InvalidOperationException($"No Buff factory is registered for definition ID '{definition.Id}'.");
            }

            if (entry.ArgsType != typeof(TArgs))
            {
                throw new InvalidOperationException(
                    $"Buff definition '{definition.Id}' requires {entry.ArgsType.Name}, but received {typeof(TArgs).Name}.");
            }

            return entry.Create(definition, args);
        }

    }
}