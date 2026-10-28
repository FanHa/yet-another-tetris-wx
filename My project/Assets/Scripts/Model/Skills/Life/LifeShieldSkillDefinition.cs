using Model;
using Model.Buffs;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Life/Life Shield")]
    public sealed class LifeShieldSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private BuffProjectile buffProjectilePrefab;
        [Header("Buff")]
        [SerializeField] private LifeShieldBuffDefinition lifeShieldBuffDefinition;
        [Header("Health cost")]
        [SerializeField] private float lifeCostPercent = 10f;
        [Header("Shield")]
        [SerializeField] private float baseShieldMultiplierPercent = 100f;
        [SerializeField] private float shieldMultiplierPercentPerResonantCell = 5f;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 100f;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;
        public LifeShieldBuffDefinition LifeShieldBuffDefinition => lifeShieldBuffDefinition;
        public float LifeCostPercent => lifeCostPercent;
        public float BaseShieldMultiplierPercent => baseShieldMultiplierPercent;
        public float ShieldMultiplierPercentPerResonantCell => shieldMultiplierPercentPerResonantCell;
        public float RequiredEnergy => requiredEnergy;

        public (
            AdditiveValueBreakdown LifeCostPercent,
            AdditiveValueBreakdown ShieldMultiplierPercent,
            float LifeCost,
            float ShieldValue,
            BuffDuration Duration) CalculateStats(float currentHealth, int resonantCellCount)
        {
            var lifeCostStats = new AdditiveValueBreakdown(lifeCostPercent);
            var shieldMultiplierStats = new AdditiveValueBreakdown(
                baseShieldMultiplierPercent,
                resonantCellCount * shieldMultiplierPercentPerResonantCell);
            float lifeCost = currentHealth * (lifeCostStats.FinalValue / 100f);
            float shieldValue = lifeCost * (shieldMultiplierStats.FinalValue / 100f);
            return (lifeCostStats, shieldMultiplierStats, lifeCost, shieldValue, lifeShieldBuffDefinition.BaseDuration);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(buffProjectilePrefab, nameof(buffProjectilePrefab));
            ValidateRequiredReference(lifeShieldBuffDefinition, nameof(lifeShieldBuffDefinition));
            ValidateFiniteNonNegative(lifeCostPercent, nameof(lifeCostPercent));
            ValidateFiniteNonNegative(baseShieldMultiplierPercent, nameof(baseShieldMultiplierPercent));
            ValidateFiniteNonNegative(shieldMultiplierPercentPerResonantCell, nameof(shieldMultiplierPercentPerResonantCell));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}