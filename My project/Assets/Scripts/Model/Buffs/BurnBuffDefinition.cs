using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Burn")]
    public sealed class BurnBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseDps;
        [SerializeField] private float baseDuration;
        [SerializeField] private float dpsPerResonantCell;
        [SerializeField] private float durationPerResonantCell;

        public float BaseDps => baseDps;
        public float BaseDuration => baseDuration;
        public float DpsPerResonantCell => dpsPerResonantCell;
        public float DurationPerResonantCell => durationPerResonantCell;

        public string DescribeEffect(float dps) => $"每秒受到{dps}点伤害";

        public (StatValue Dps, StatValue Duration) CalculateStats(int resonantCellCount)
        {
            return (
                new StatValue("灼烧每秒伤害", baseDps, resonantCellCount * dpsPerResonantCell),
                new StatValue("灼烧持续时间", baseDuration, resonantCellCount * durationPerResonantCell)
            );
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(DisplayName)} is not assigned.", this);
            }

            ValidateFiniteNonNegative(baseDps, nameof(baseDps));
            ValidateFinitePositive(baseDuration, nameof(baseDuration));
            ValidateFiniteNonNegative(dpsPerResonantCell, nameof(dpsPerResonantCell));
            ValidateFiniteNonNegative(durationPerResonantCell, nameof(durationPerResonantCell));
        }

        private void ValidateFiniteNonNegative(float value, string fieldName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and non-negative.", this);
            }
        }

        private void ValidateFinitePositive(float value, string fieldName)
        {
            if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and greater than zero.", this);
            }
        }
#endif
    }
}