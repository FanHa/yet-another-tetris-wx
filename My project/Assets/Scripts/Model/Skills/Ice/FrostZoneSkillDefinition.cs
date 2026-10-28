using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Ice/Frost Zone")]
    public sealed class FrostZoneSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private FrostZone zonePrefab;
        [Header("Area")]
        [SerializeField] private float baseRadius = 1.5f;
        [SerializeField] private float radiusPerResonantCell = 0.2f;
        [SerializeField] private float baseDuration = 5f;
        [SerializeField] private float durationPerResonantCell = 1f;
        [Header("Damage")]
        [SerializeField] private float baseDamage = 8f;
        [SerializeField] private float damagePerResonantCell = 3f;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 120f;

        public FrostZone ZonePrefab => zonePrefab;
        public float BaseRadius => baseRadius;
        public float RadiusPerResonantCell => radiusPerResonantCell;
        public float BaseDuration => baseDuration;
        public float DurationPerResonantCell => durationPerResonantCell;
        public float BaseDamage => baseDamage;
        public float DamagePerResonantCell => damagePerResonantCell;
        public float RequiredEnergy => requiredEnergy;

        public (
            AdditiveValueBreakdown Radius,
            AdditiveValueBreakdown Duration,
            AdditiveValueBreakdown Damage) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseRadius, resonantCellCount * radiusPerResonantCell),
                new AdditiveValueBreakdown(baseDuration, resonantCellCount * durationPerResonantCell),
                new AdditiveValueBreakdown(baseDamage, resonantCellCount * damagePerResonantCell));
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
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}