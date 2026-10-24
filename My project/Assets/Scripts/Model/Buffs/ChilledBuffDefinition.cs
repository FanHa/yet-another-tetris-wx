using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Chilled")]
    public sealed class ChilledBuffDefinition : BuffDefinition
    {
        [SerializeField] private float durationPerResonance;
        [SerializeField] private int baseMoveSlowPercent;
        [SerializeField] private int moveSlowPercentPerResonantCell;
        [SerializeField] private int baseAttackSlowPercent;
        [SerializeField] private int attackSlowPercentPerResonantCell;
        [SerializeField] private int baseActionSlowPercent;
        [SerializeField] private int actionSlowPercentPerResonantCell;

        public (AdditiveValueBreakdown Duration, AdditiveValueBreakdown MoveSlowPercent, AdditiveValueBreakdown AttackSlowPercent, AdditiveValueBreakdown ActionSlowPercent) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(BaseDuration.Seconds, resonantCellCount * durationPerResonance),
                new AdditiveValueBreakdown(baseMoveSlowPercent, resonantCellCount * moveSlowPercentPerResonantCell),
                new AdditiveValueBreakdown(baseAttackSlowPercent, resonantCellCount * attackSlowPercentPerResonantCell),
                new AdditiveValueBreakdown(baseActionSlowPercent, resonantCellCount * actionSlowPercentPerResonantCell)
            );
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"{StatBreakdownFormatter.Format(StatLabel.ChillDuration, stats.Duration)}\n{StatBreakdownFormatter.Format(StatLabel.MovementSpeedReductionPercent, stats.MoveSlowPercent)}\n{StatBreakdownFormatter.Format(StatLabel.AttackSpeedReductionPercent, stats.AttackSlowPercent)}\n{StatBreakdownFormatter.Format(StatLabel.ActionRateReductionPercent, stats.ActionSlowPercent)}";
        }

        public string DescribeEffect(float duration, int moveSlowPercent, int attackSlowPercent, int actionSlowPercent)
        {
            return $"移速-{moveSlowPercent}%，攻速-{attackSlowPercent}%，动作速率-{actionSlowPercent}%，持续{duration}秒";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateTimedBaseDuration();
            ValidateFiniteNonNegative(durationPerResonance, nameof(durationPerResonance));
            ValidateNonNegative(baseMoveSlowPercent, nameof(baseMoveSlowPercent));
            ValidateNonNegative(moveSlowPercentPerResonantCell, nameof(moveSlowPercentPerResonantCell));
            ValidateNonNegative(baseAttackSlowPercent, nameof(baseAttackSlowPercent));
            ValidateNonNegative(attackSlowPercentPerResonantCell, nameof(attackSlowPercentPerResonantCell));
            ValidateNonNegative(baseActionSlowPercent, nameof(baseActionSlowPercent));
            ValidateNonNegative(actionSlowPercentPerResonantCell, nameof(actionSlowPercentPerResonantCell));
        }

        private void ValidateFiniteNonNegative(float value, string fieldName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and non-negative.", this);
            }
        }

        private void ValidateNonNegative(int value, string fieldName)
        {
            if (value < 0)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be non-negative.", this);
            }
        }
#endif
    }
}