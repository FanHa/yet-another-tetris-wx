using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Wind Shift")]
    public sealed class WindShiftBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration = -1f;
        [SerializeField] private float baseAttackRangeBonus;
        [SerializeField] private float attackRangePerResonantCell;

        public float Duration => duration;

        public (StatValue AttackRangeBonus, StatValue Duration) CalculateStats(int resonantCellCount) =>
            (
                new StatValue("攻击距离提升", baseAttackRangeBonus, resonantCellCount * attackRangePerResonantCell),
                new StatValue("风形态持续时间", duration)
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"攻击时额外造成等于当前最终攻击距离的伤害。\n{stats.AttackRangeBonus}\n{stats.Duration}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }

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