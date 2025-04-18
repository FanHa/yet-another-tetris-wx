using System;
using UnityEngine;

namespace Units
{
    [System.Serializable]
    public class Attributes
    {
        [SerializeField] private Attribute moveSpeed;
        [SerializeField] private Attribute attackPower;
        [SerializeField] private Attribute maxHealth;
        [SerializeField] private Attribute attacksPerTenSeconds;

        public Attribute MoveSpeed => moveSpeed;
        public Attribute AttackPower => attackPower;
        public Attribute MaxHealth => maxHealth;
        public Attribute AttacksPerTenSeconds => attacksPerTenSeconds;

        public float AttackTargetNumber; // 攻击目标数量
        public float AttackRange; // 攻击范围
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
            moveSpeed = new Attribute(2);
            attackPower = new Attribute(10);
            maxHealth = new Attribute(100);
            attacksPerTenSeconds = new Attribute(2.5f);
            CurrentHealth = MaxHealth.finalValue;
        }
    }
}