using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Electric/Chain Lightning")]
    public sealed class ChainLightningSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private ChainLightning projectilePrefab;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 50f;
        [Header("Chain")]
        [SerializeField] private float baseDamage = 5f;
        [SerializeField] private float damagePerResonantCell = 4f;
        [SerializeField] private float damageIncreasePercentage = 20f;
        [SerializeField] private int maxBounces = 2;
        [SerializeField] private float range = 5f;
        [Header("Energy disruption")]
        [SerializeField] private float baseEnergyDisruptionAmount = 5f;
        [SerializeField] private float energyDisruptionAmountPerResonantCell = 8f;

        public ChainLightning ProjectilePrefab => projectilePrefab;
        public float RequiredEnergy => requiredEnergy;
        public float BaseDamage => baseDamage;
        public float DamagePerResonantCell => damagePerResonantCell;
        public float DamageIncreasePercentage => damageIncreasePercentage;
        public int MaxBounces => maxBounces;
        public float Range => range;
        public float BaseEnergyDisruptionAmount => baseEnergyDisruptionAmount;
        public float EnergyDisruptionAmountPerResonantCell => energyDisruptionAmountPerResonantCell;

        public (AdditiveValueBreakdown InitialDamage, float EnergyDisruptionAmount) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseDamage, resonantCellCount * damagePerResonantCell),
                baseEnergyDisruptionAmount + resonantCellCount * energyDisruptionAmountPerResonantCell);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
            ValidateFiniteNonNegative(baseDamage, nameof(baseDamage));
            ValidateFiniteNonNegative(damagePerResonantCell, nameof(damagePerResonantCell));
            ValidateFiniteNonNegative(damageIncreasePercentage, nameof(damageIncreasePercentage));
            ValidateFinitePositive(range, nameof(range));
            ValidateFiniteNonNegative(baseEnergyDisruptionAmount, nameof(baseEnergyDisruptionAmount));
            ValidateFiniteNonNegative(energyDisruptionAmountPerResonantCell, nameof(energyDisruptionAmountPerResonantCell));
        }
#endif
    }
}