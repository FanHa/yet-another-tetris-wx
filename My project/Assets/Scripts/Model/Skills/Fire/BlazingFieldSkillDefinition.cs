using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Fire/Blazing Field")]
    public sealed class BlazingFieldSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private BlazingField zonePrefab;
        [Header("Area")]
        [SerializeField] private float baseRadius = 1.5f;
        [SerializeField] private float radiusPerResonantCell = 0.2f;
        [SerializeField] private float baseDuration = 5f;
        [SerializeField] private float durationPerResonantCell = 1f;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 120f;

        public BlazingField ZonePrefab => zonePrefab;
        public float BaseRadius => baseRadius;
        public float RadiusPerResonantCell => radiusPerResonantCell;
        public float BaseDuration => baseDuration;
        public float DurationPerResonantCell => durationPerResonantCell;
        public float RequiredEnergy => requiredEnergy;

        public (AdditiveValueBreakdown Radius, AdditiveValueBreakdown Duration) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseRadius, resonantCellCount * radiusPerResonantCell),
                new AdditiveValueBreakdown(baseDuration, resonantCellCount * durationPerResonantCell));
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
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}