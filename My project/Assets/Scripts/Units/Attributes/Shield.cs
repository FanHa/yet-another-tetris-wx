using System;
using UnityEngine;

namespace Units
{
    public class Shield
    {
        public float Value { get; private set; }
        public object Source { get; }
        public event Action<Shield> OnBroken;

        public Shield(object source, float value)
        {
            Source = source;
            Value = value;
        }

        public float Absorb(float damage)
        {
            float absorbed = Mathf.Min(Value, damage);
            Value -= absorbed;
            if (Value <= 0)
            {
                OnBroken?.Invoke(this);
            }
            return absorbed;
        }
    }
}
