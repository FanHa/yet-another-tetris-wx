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
        [SerializeField] private Attribute energyPerTick;
        
        public Attribute MoveSpeed => moveSpeed;
        public Attribute AttackPower => attackPower;
        public Attribute MaxHealth => maxHealth;
        public Attribute AttacksPerTenSeconds => attacksPerTenSeconds;
        public Attribute EnergyPerTick => energyPerTick;

        public float AttackTargetNumber; // 攻击目标数量
        public float AttackRange; // 攻击范围
        public bool IsRanged; // 是否为远程单位
        public float RangeAttackDamagePercentage;

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
            moveSpeed = new Attribute("移速", 2);
            attackPower = new Attribute("攻击力", 10);
            maxHealth = new Attribute("生命", 100);
            attacksPerTenSeconds = new Attribute("攻速", 2.5f);
            energyPerTick = new Attribute("能量回复", 5);
            CurrentHealth = MaxHealth.finalValue;
        }
    }
}