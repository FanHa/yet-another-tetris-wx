using Model;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Curse/Power Curse")]
    public sealed class PowerCurseSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private Units.Projectiles.PowerCurseZone zonePrefab;
        [SerializeField] private float baseRadius = 5f;
        [SerializeField] private float radiusPerResonantCell = 0.3f;
        [SerializeField] private float baseZoneLifetime = 15f;
        [SerializeField] private float zoneLifetimePerResonantCell = 0.5f;

        public Units.Projectiles.PowerCurseZone ZonePrefab => zonePrefab;
        public float BaseRadius => baseRadius;
        public float RadiusPerResonantCell => radiusPerResonantCell;
        public float BaseZoneLifetime => baseZoneLifetime;
        public float ZoneLifetimePerResonantCell => zoneLifetimePerResonantCell;

        public (AdditiveValueBreakdown Radius, AdditiveValueBreakdown ZoneLifetime) CalculateStats(int resonantCellCount)
        {
            return (
            new AdditiveValueBreakdown(baseRadius, resonantCellCount * radiusPerResonantCell),
            new AdditiveValueBreakdown(baseZoneLifetime, resonantCellCount * zoneLifetimePerResonantCell));
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
            ValidateFinitePositive(baseZoneLifetime, nameof(baseZoneLifetime));
            ValidateFiniteNonNegative(zoneLifetimePerResonantCell, nameof(zoneLifetimePerResonantCell));
        }
#endif
    }
}