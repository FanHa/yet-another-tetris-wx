using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Ice Breaker")]
    public sealed class IceBreakerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseExtraDamage;
        [SerializeField] private float extraDamagePerResonantCell;
        [SerializeField] private float percentSlowMultiplier;

        public (AdditiveValueBreakdown BaseExtraDamage, AdditiveValueBreakdown PercentSlowMultiplier, string Duration) CalculateStats(int resonantCellCount) =>
            (
                new AdditiveValueBreakdown(baseExtraDamage, resonantCellCount * extraDamagePerResonantCell),
                new AdditiveValueBreakdown(percentSlowMultiplier),
                BaseDuration.ToDisplayString("破冰持续时间")
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"命中时移除目标全部 Chilled。额外伤害 = 基础额外伤害 + 减速百分比伤害倍率 * 减速百分比\n{StatBreakdownFormatter.Format(StatLabel.BaseExtraDamage, stats.BaseExtraDamage)}\n{StatBreakdownFormatter.Format(StatLabel.SlowBasedDamageMultiplier, stats.PercentSlowMultiplier)}\n{stats.Duration}";
        }

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