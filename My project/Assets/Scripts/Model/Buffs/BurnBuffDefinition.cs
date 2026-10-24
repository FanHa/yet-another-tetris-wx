using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Burn")]
    public sealed class BurnBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseDps;
        [SerializeField] private float dpsPerResonantCell;
        [SerializeField] private float durationPerResonance;

        public float BaseDps => baseDps;
        public float DpsPerResonantCell => dpsPerResonantCell;
        public float DurationPerResonance => durationPerResonance;

        public string DescribeEffect(float dps) => $"每秒受到{dps}点伤害";

        public (AdditiveValueBreakdown Dps, AdditiveValueBreakdown Duration) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseDps, resonantCellCount * dpsPerResonantCell),
                new AdditiveValueBreakdown(BaseDuration.Seconds, resonantCellCount * durationPerResonance)
            );
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateFiniteNonNegative(baseDps, nameof(baseDps));
            ValidateTimedBaseDuration();
            ValidateFiniteNonNegative(dpsPerResonantCell, nameof(dpsPerResonantCell));
            ValidateFiniteNonNegative(durationPerResonance, nameof(durationPerResonance));
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