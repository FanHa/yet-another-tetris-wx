using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Fire/Flame Ring")]
    public sealed class FlameRingSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private FlameRing zonePrefab;
        [Header("Area")]
        [SerializeField] private float areaDuration = -1f;
        [SerializeField] private float baseRadius = 1f;
        [SerializeField] private float radiusPerResonantCell = 0.2f;

        public FlameRing ZonePrefab => zonePrefab;
        public float AreaDuration => areaDuration;
        public float BaseRadius => baseRadius;
        public float RadiusPerResonantCell => radiusPerResonantCell;

        public (AdditiveValueBreakdown Radius, float AreaDuration) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseRadius, resonantCellCount * radiusPerResonantCell),
                areaDuration);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(zonePrefab, nameof(zonePrefab));
            if (areaDuration < -1f || float.IsNaN(areaDuration) || float.IsInfinity(areaDuration))
            {
                Debug.LogError($"{nameof(areaDuration)} must be finite and at least -1.", this);
            }

            ValidateFinitePositive(baseRadius, nameof(baseRadius));
            ValidateFiniteNonNegative(radiusPerResonantCell, nameof(radiusPerResonantCell));
        }
#endif
    }
}