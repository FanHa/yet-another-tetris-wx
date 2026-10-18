using System;
using System.Collections.Generic;

namespace Units.States
{
    public sealed class UnitStateHandler
    {
        private readonly List<IUnitState> states = new();
        private readonly IReadOnlyList<IUnitState> statesReadOnly;

        public UnitStateHandler()
        {
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
            return true;
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
    }
}