using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Wind Knockback")]
    public sealed class WindKnockbackBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseKnockbackDistance;
        [SerializeField] private float knockbackDistancePerResonantCell;
        [SerializeField] private float maxKnockbackDistance;

        public (AdditiveValueBreakdown KnockbackDistance, AdditiveValueBreakdown MaxKnockbackDistance, BuffDuration Duration) CalculateStats(int resonantCellCount) =>
            (
                new AdditiveValueBreakdown(baseKnockbackDistance, resonantCellCount * knockbackDistancePerResonantCell),
                new AdditiveValueBreakdown(maxKnockbackDistance),
                BaseDuration
            );

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateFiniteNonNegative(baseKnockbackDistance, nameof(baseKnockbackDistance));
            ValidateFiniteNonNegative(knockbackDistancePerResonantCell, nameof(knockbackDistancePerResonantCell));
            ValidateFiniteNonNegative(maxKnockbackDistance, nameof(maxKnockbackDistance));
        }

        private void ValidateFiniteNonNegative(float value, string fieldName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and non-negative.", this);
            }
        }
#endif
    }
}