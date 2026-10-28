using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Wind Shift")]
    public sealed class WindShiftBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseAttackRangeBonus;
        [SerializeField] private float attackRangePerResonantCell;

        public (AdditiveValueBreakdown AttackRangeBonus, BuffDuration Duration) CalculateStats(int resonantCellCount) =>
            (
                new AdditiveValueBreakdown(baseAttackRangeBonus, resonantCellCount * attackRangePerResonantCell),
                BaseDuration
            );

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateFiniteNonNegative(baseAttackRangeBonus, nameof(baseAttackRangeBonus));
            ValidateFiniteNonNegative(attackRangePerResonantCell, nameof(attackRangePerResonantCell));
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