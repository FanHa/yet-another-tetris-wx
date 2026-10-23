using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Wind Knockback")]
    public sealed class WindKnockbackBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseKnockbackDistance;
        [SerializeField] private float knockbackDistancePerResonantCell;
        [SerializeField] private float maxKnockbackDistance;

        public (StatBreakdownDisplay KnockbackDistance, StatBreakdownDisplay MaxKnockbackDistance, string Duration) CalculateStats(int resonantCellCount) =>
            (
                new StatBreakdownDisplay(StatLabel.KnockbackDistance, baseKnockbackDistance, resonantCellCount * knockbackDistancePerResonantCell),
                new StatBreakdownDisplay(StatLabel.MaxKnockbackDistance, maxKnockbackDistance),
                BaseDuration.ToDisplayString("轻风击退持续时间")
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            float distance = Mathf.Min(stats.KnockbackDistance.FinalValue, stats.MaxKnockbackDistance.FinalValue);
            return $"普攻命中时将目标击退{distance:F2}（最多{stats.MaxKnockbackDistance.FinalValue:F2}）\n{stats.KnockbackDistance}\n{stats.MaxKnockbackDistance}\n{stats.Duration}";
        }

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