using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Attribute energyPerSecond;

        public Attribute MoveSpeed => moveSpeed;
        public Attribute AttackPower => attackPower;
        public Attribute MaxHealth => maxHealth;
        public Attribute AttacksPerTenSeconds => attacksPerTenSeconds;
        public Attribute EnergyPerSecond => energyPerSecond;
        public float AttackRange; // 攻击范围
        [SerializeField] private float RangedThreshold;
        public bool IsRanged => AttackRange >= RangedThreshold;

        private float currentHealth;
        public float CurrentHealth
        {
            get => currentHealth;
            private set
            {
                currentHealth = Mathf.Clamp(value, 0, MaxHealth.finalValue);
                OnHealthChanged?.Invoke(currentHealth, MaxHealth.finalValue);
            }
        }
        public event Action<float, float> OnHealthChanged;

        public float ShieldValue => shields.Sum(s => s.Value);
        private List<Shield> shields;
        public event Action<float> OnShieldChanged;

        public Attributes(
            float moveSpeedBase,
            float attackPowerBase,
            float maxHealthBase,
            float attacksPerTenSecondsBase,
            float energyPerSecondBase,
            float attackRange
        )
        {
            moveSpeed = new Attribute("移速", moveSpeedBase);
            attackPower = new Attribute("攻击力", attackPowerBase);
            maxHealth = new Attribute("生命", maxHealthBase);
            attacksPerTenSeconds = new Attribute("攻速", attacksPerTenSecondsBase);
            energyPerSecond = new Attribute("能量回复", energyPerSecondBase);

            AttackRange = attackRange;

            shields = new List<Shield>();
            CurrentHealth = MaxHealth.finalValue;
        }

        public void AddShield(Shield shield)
        {
            shields.Add(shield);
            OnShieldChanged?.Invoke(ShieldValue);
        }

        public void RemoveShield(Shield shield)
        {
            if (shields.Remove(shield))
                OnShieldChanged?.Invoke(ShieldValue);
        }


        public void TakeDamage(float damage)
        {
            for (int i = 0; i < shields.Count && damage > 0;)
            {
                var shield = shields[i];
                float absorbed = shield.Absorb(damage);
                damage -= absorbed;
                if (shield.Value <= 0)
                {
                    RemoveShield(shield);
                    // shield.OnBroken 事件会自动通知Buff
                }
                else
                {
                    i++;
                }
            }
            if (damage > 0)
                CurrentHealth -= damage;
            OnShieldChanged?.Invoke(ShieldValue);
        }
        
        public void RefillHealthToMax()
        {
            CurrentHealth = MaxHealth.finalValue;
        }
    }
}