using System;
using System.Collections.Generic;

namespace Units.States
{
    public readonly struct UnitStateHandlerArgs
    {
        public static UnitStateHandlerArgs Default { get; } = new(0.2f, 1f);

        public float ChargeInterval { get; }
        public float EnergyDecayPerChargeableState { get; }

        public UnitStateHandlerArgs(float chargeInterval, float energyDecayPerChargeableState)
        {
            if (float.IsNaN(chargeInterval) || float.IsInfinity(chargeInterval) || chargeInterval <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(chargeInterval), "Must be positive and finite.");
            }

            if (float.IsNaN(energyDecayPerChargeableState)
                || float.IsInfinity(energyDecayPerChargeableState)
                || energyDecayPerChargeableState < 0f
                || energyDecayPerChargeableState > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(energyDecayPerChargeableState), "Must be finite and between 0 and 1.");
            }

            ChargeInterval = chargeInterval;
            EnergyDecayPerChargeableState = energyDecayPerChargeableState;
        }
    }

    public sealed class UnitStateHandler
    {
        private sealed class ChargeableStateEntry
        {
            public readonly IEnergyChargeableState State;
            public float ElapsedTime;

            public ChargeableStateEntry(IEnergyChargeableState state)
            {
                State = state;
            }
        }

        private readonly UnitStateHandlerArgs args;
        private readonly List<IUnitState> states = new();
        private readonly List<ChargeableStateEntry> chargeableStates = new();
        private readonly IReadOnlyList<IUnitState> statesReadOnly;

        public UnitStateHandler(UnitStateHandlerArgs args)
        {
            this.args = args;
            statesReadOnly = states.AsReadOnly();
        }

        public bool AddState(IUnitState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (FindStateIndex(state) >= 0)
            {
                return false;
            }

            states.Add(state);
            if (state is IEnergyChargeableState chargeableState)
            {
                chargeableStates.Add(new ChargeableStateEntry(chargeableState));
            }

            return true;
        }

        public bool RemoveState(IUnitState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            int index = FindStateIndex(state);
            if (index < 0)
            {
                return false;
            }

            states.RemoveAt(index);
            if (state is IEnergyChargeableState chargeableState)
            {
                int chargeableIndex = FindChargeableStateIndex(chargeableState);
                if (chargeableIndex >= 0)
                {
                    chargeableStates.RemoveAt(chargeableIndex);
                }
            }

            return true;
        }

        public void AccumulateEnergy(float deltaTime, float baseEnergyPerSecond)
        {
            if (chargeableStates.Count == 0)
            {
                return;
            }

            float decayFactor = (float)Math.Pow(
                args.EnergyDecayPerChargeableState,
                Math.Max(0, chargeableStates.Count - 1));
            for (int index = 0; index < chargeableStates.Count; index++)
            {
                ChargeableStateEntry entry = chargeableStates[index];
                entry.ElapsedTime += deltaTime;
                if (entry.ElapsedTime < args.ChargeInterval)
                {
                    continue;
                }

                float elapsedEnergyTime = (float)Math.Floor(entry.ElapsedTime / args.ChargeInterval) * args.ChargeInterval;
                entry.ElapsedTime -= elapsedEnergyTime;
                float energyToDistribute = baseEnergyPerSecond * elapsedEnergyTime * decayFactor;
                entry.State.AddEnergy(energyToDistribute);
            }
        }

        public IReadOnlyList<IUnitState> GetStatesReadOnly()
        {
            return statesReadOnly;
        }

        public IReadOnlyList<TState> GetStates<TState>() where TState : class, IUnitState
        {
            var matchingStates = new List<TState>();
            for (int index = 0; index < states.Count; index++)
            {
                if (states[index] is TState matchingState)
                {
                    matchingStates.Add(matchingState);
                }
            }

            return matchingStates.AsReadOnly();
        }

        private int FindStateIndex(IUnitState state)
        {
            for (int index = 0; index < states.Count; index++)
            {
                if (ReferenceEquals(states[index], state))
                {
                    return index;
                }
            }

            return -1;
        }

        private int FindChargeableStateIndex(IEnergyChargeableState state)
        {
            for (int index = 0; index < chargeableStates.Count; index++)
            {
                if (ReferenceEquals(chargeableStates[index].State, state))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}