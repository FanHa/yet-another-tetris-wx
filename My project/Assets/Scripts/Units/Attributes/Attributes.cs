using System;
using UnityEngine;

namespace Units
{
    [System.Serializable]
    public class Attributes
    {
        public Attribute MoveSpeed { get; private set; } = new Attribute(3f);
        public Attribute AttackPower { get; private set; } = new Attribute(10f);
        public Attribute MaxHealth { get; private set; } = new Attribute(100f);
        public Attribute AttacksPerTenSeconds { get; private set; } = new Attribute(3f);
        public float AttackTargetNumber = 1; // 攻击目标数量
        public float AttackRange = 0.5f; // 攻击范围
        public bool IsRanged; // 是否为远程单位

        private float currentHealth;
        public float CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = Mathf.Clamp(value, 0, MaxHealth.finalValue);
                OnHealthChanged?.Invoke(currentHealth, MaxHealth.finalValue);
            }
        }
        public event Action<float, float> OnHealthChanged;


        public Attributes()
        {
            CurrentHealth = MaxHealth.finalValue;
        }
    }
}