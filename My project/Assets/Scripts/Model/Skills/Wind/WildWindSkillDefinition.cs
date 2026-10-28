using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Wind/Wild Wind")]
    public sealed class WildWindSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private WildWind projectilePrefab;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 100f;
        [Header("Area")]
        [SerializeField] private float damage = 5f;
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private float duration = 2f;
        [Header("Resonance bonuses")]
        [SerializeField] private float damageAdditionPerResonantCell = 5f;
        [SerializeField] private float radiusPerResonantCell = 0.1f;
        [SerializeField] private float durationPerResonantCell = 0.25f;

        public WildWind ProjectilePrefab => projectilePrefab;
        public float RequiredEnergy => requiredEnergy;
        public float Damage => damage;
        public float Radius => radius;
        public float Duration => duration;
        public float DamageAdditionPerResonantCell => damageAdditionPerResonantCell;
        public float RadiusPerResonantCell => radiusPerResonantCell;
        public float DurationPerResonantCell => durationPerResonantCell;

        public (AdditiveValueBreakdown Damage, AdditiveValueBreakdown Radius, AdditiveValueBreakdown Duration) CalculateStats(int resonantCellCount) =>
            (
                new AdditiveValueBreakdown(damage, resonantCellCount * damageAdditionPerResonantCell),
                new AdditiveValueBreakdown(radius, resonantCellCount * radiusPerResonantCell),
                new AdditiveValueBreakdown(duration, resonantCellCount * durationPerResonantCell)
            );

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
            ValidateFiniteNonNegative(damage, nameof(damage));
            ValidateFinitePositive(radius, nameof(radius));
            ValidateFiniteNonNegative(duration, nameof(duration));
            ValidateFiniteNonNegative(damageAdditionPerResonantCell, nameof(damageAdditionPerResonantCell));
            ValidateFiniteNonNegative(radiusPerResonantCell, nameof(radiusPerResonantCell));
            ValidateFiniteNonNegative(durationPerResonantCell, nameof(durationPerResonantCell));
        }
#endif
    }
}