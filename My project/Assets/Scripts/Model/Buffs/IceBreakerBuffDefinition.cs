using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Ice Breaker")]
    public sealed class IceBreakerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseExtraDamage;
        [SerializeField] private float extraDamagePerResonantCell;
        [SerializeField] private float percentSlowMultiplier;

        public (AdditiveValueBreakdown BaseExtraDamage, AdditiveValueBreakdown PercentSlowMultiplier) CalculateStats(int resonantCellCount) =>
            (
                new AdditiveValueBreakdown(baseExtraDamage, resonantCellCount * extraDamagePerResonantCell),
                new AdditiveValueBreakdown(percentSlowMultiplier)
            );

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateFiniteNonNegative(baseExtraDamage, nameof(baseExtraDamage));
            ValidateFiniteNonNegative(extraDamagePerResonantCell, nameof(extraDamagePerResonantCell));
            ValidateFiniteNonNegative(percentSlowMultiplier, nameof(percentSlowMultiplier));
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