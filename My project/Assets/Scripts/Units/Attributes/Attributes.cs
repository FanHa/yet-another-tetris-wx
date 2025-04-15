using System;
using UnityEngine;

namespace Units
{
    [System.Serializable]
    public class Attributes
    {
        [SerializeField] private Attribute moveSpeed = new Attribute(3f);
        [SerializeField] private Attribute attackPower = new Attribute(10f);
        [SerializeField] private Attribute maxHealth = new Attribute(100f);
        [SerializeField] private Attribute attacksPerTenSeconds = new Attribute(3f);

        public Attribute MoveSpeed => moveSpeed;
        public Attribute AttackPower => attackPower;
        public Attribute MaxHealth => maxHealth;
        public Attribute AttacksPerTenSeconds => attacksPerTenSeconds;

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