using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Ice Breaker")]
    public sealed class IceBreakerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration = -1f;
        [SerializeField] private float baseExtraDamage;
        [SerializeField] private float extraDamagePerResonantCell;
        [SerializeField] private float percentSlowMultiplier;

        public float Duration => duration;

        public (StatValue BaseExtraDamage, StatValue PercentSlowMultiplier, StatValue Duration) CalculateStats(int resonantCellCount) =>
            (
                new StatValue("基础额外伤害", baseExtraDamage, resonantCellCount * extraDamagePerResonantCell),
                new StatValue("减速百分比伤害倍率", percentSlowMultiplier),
                new StatValue("破冰持续时间", duration)
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"命中时移除目标全部 Chilled。额外伤害 = 基础额外伤害 + 减速百分比伤害倍率 * 减速百分比\n{stats.BaseExtraDamage}\n{stats.PercentSlowMultiplier}\n{stats.Duration}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }

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