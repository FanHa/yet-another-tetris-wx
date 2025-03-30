using System;
using UnityEngine;

namespace Units
{
    public abstract class Debuff
    {
        public abstract float Duration(); // Duration of the debuff
    
        public abstract string Name();
        private float startTime = 0; // Time when the debuff was applied

        public abstract void ApplyEffect(Unit unit);

        public abstract string Description();

        public void Start(){
            startTime = Time.time; // Record the time when the debuff was applied
        }

        public bool IsExpired()
        {
            // Check if the debuff has expired based on its duration and the time it was applied
            return Time.time - startTime >= Duration();
        }
    }
}
