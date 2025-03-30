using System;
using UnityEngine;

namespace Units
{
    public abstract class Debuff
    {
        public float Duration { get; private set; } // Duration of the debuff
        public float Interval = 1;

        public abstract void ApplyEffect(Unit unit);

        public abstract string Description();

        public override bool Equals(object obj)
        {
            if (obj is Debuff otherDebuff)
            {
                return GetType() == otherDebuff.GetType();

            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetType()?.GetHashCode() ?? 0;
        }
    }
}
