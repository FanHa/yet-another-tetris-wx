using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Electric/Electric Field")]
    public sealed class ElectricFieldSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private ElectricFieldZone zonePrefab;
        [Header("Area")]
        [SerializeField] private float baseRadius = 2f;
        [SerializeField] private float radiusPerResonantCell = 0.2f;
        [SerializeField] private float baseDuration = 3f;
        [SerializeField] private float durationPerResonantCell = 0.2f;
        [Header("Damage")]
        [SerializeField] private float baseDamage = 4f;
        [SerializeField] private float damagePerResonantCell = 4f;
        [SerializeField] private float baseEnergyDisruptionAmount = 5f;
        [SerializeField] private float energyDisruptionAmountPerResonantCell = 5f;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 90f;

        public ElectricFieldZone ZonePrefab => zonePrefab;
        public float BaseRadius => baseRadius;
        public float RadiusPerResonantCell => radiusPerResonantCell;
        public float BaseDuration => baseDuration;
        public float DurationPerResonantCell => durationPerResonantCell;
        public float BaseDamage => baseDamage;
        public float DamagePerResonantCell => damagePerResonantCell;
        public float BaseEnergyDisruptionAmount => baseEnergyDisruptionAmount;
        public float EnergyDisruptionAmountPerResonantCell => energyDisruptionAmountPerResonantCell;
        public float RequiredEnergy => requiredEnergy;

        public (
            AdditiveValueBreakdown Radius,
            AdditiveValueBreakdown Duration,
            AdditiveValueBreakdown Damage,
            AdditiveValueBreakdown EnergyDisruptionAmount) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseRadius, resonantCellCount * radiusPerResonantCell),
                new AdditiveValueBreakdown(baseDuration, resonantCellCount * durationPerResonantCell),
                new AdditiveValueBreakdown(baseDamage, resonantCellCount * damagePerResonantCell),
                new AdditiveValueBreakdown(baseEnergyDisruptionAmount, resonantCellCount * energyDisruptionAmountPerResonantCell));
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(zonePrefab, nameof(zonePrefab));
            ValidateFinitePositive(baseRadius, nameof(baseRadius));
            ValidateFiniteNonNegative(radiusPerResonantCell, nameof(radiusPerResonantCell));
            ValidateFinitePositive(baseDuration, nameof(baseDuration));
            ValidateFiniteNonNegative(durationPerResonantCell, nameof(durationPerResonantCell));
            ValidateFiniteNonNegative(baseDamage, nameof(baseDamage));
            ValidateFiniteNonNegative(damagePerResonantCell, nameof(damagePerResonantCell));
            ValidateFiniteNonNegative(baseEnergyDisruptionAmount, nameof(baseEnergyDisruptionAmount));
            ValidateFiniteNonNegative(energyDisruptionAmountPerResonantCell, nameof(energyDisruptionAmountPerResonantCell));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}