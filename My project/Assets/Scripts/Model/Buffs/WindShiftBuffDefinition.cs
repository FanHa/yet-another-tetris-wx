using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Wind Shift")]
    public sealed class WindShiftBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseAttackRangeBonus;
        [SerializeField] private float attackRangePerResonantCell;

        public (AdditiveValueBreakdown AttackRangeBonus, string Duration) CalculateStats(int resonantCellCount) =>
            (
                new AdditiveValueBreakdown(baseAttackRangeBonus, resonantCellCount * attackRangePerResonantCell),
                BaseDuration.ToDisplayString("风形态持续时间")
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"攻击时额外造成等于当前最终攻击距离的伤害。\n{StatBreakdownFormatter.Format(StatLabel.AttackRangeIncrease, stats.AttackRangeBonus)}\n{stats.Duration}";
        }

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