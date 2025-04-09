using System;
using UnityEngine;

namespace Units.Buffs
{
    public abstract class Buff
    {
        public abstract float Duration(); // Duration of the debuff
    
        public abstract string Name();
        private float startTime = 0; // Time when the debuff was applied

        public abstract void Apply(Unit unit);
        public abstract void Remove(Unit unit); // Method to remove the debuff from the unit
        public abstract void Affect(Unit unit); // Method to apply the debuff effect on the unit
        public abstract string Description();

        public void RefreshDuration(){
            startTime = Time.time; // Record the time when the debuff was applied
        }

        public bool IsExpired()
        {
            // Check if the debuff has expired based on its duration and the time it was applied
            return Time.time - startTime >= Duration();
        }
    }
}
