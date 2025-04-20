using System;
using UnityEngine;

namespace Units.Buffs
{
    public abstract class Buff
    {
        protected float durationSeconds; // Duration of the buff
        public float DurationRevisePercentage = 100f;

        public virtual float Duration() => durationSeconds * DurationRevisePercentage / 100; // Virtual method for duration
    
        public abstract string Name();
        private float startTime = 0; // Time when the buff was applied

        public abstract void Apply(Unit unit);
        public abstract void Remove(Unit unit); // Method to remove the buff from the unit
        public abstract void Affect(Unit unit); // Method to apply the buff effect on the unit
        
        public abstract string Description();

        public abstract Type TetriCellType { get; } // Property to store TetriCell type as a Type

        public void RefreshDuration(){
            startTime = Time.time; // Record the time when the buff was applied
        }

        public bool IsExpired()
        {
            // Check if the buff has expired based on its duration and the time it was applied
            return Time.time - startTime >= Duration();
        }
    }
}
