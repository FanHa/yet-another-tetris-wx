using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Attack Boost")]
    public sealed class AttackBoostBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseAtkSpeedPercent;
        [SerializeField] private float atkSpeedPercentPerResonantCell;

        public (BuffDuration Duration, AdditiveValueBreakdown AtkSpeedPercent) CalculateStats(int resonantCellCount) =>
            (
                BaseDuration,
                new AdditiveValueBreakdown(baseAtkSpeedPercent, resonantCellCount * atkSpeedPercentPerResonantCell)
            );

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateTimedBaseDuration();

            ValidateFiniteNonNegative(baseAtkSpeedPercent, nameof(baseAtkSpeedPercent));
            ValidateFiniteNonNegative(atkSpeedPercentPerResonantCell, nameof(atkSpeedPercentPerResonantCell));
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