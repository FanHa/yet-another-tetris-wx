using UnityEngine;
using Units.Skills;

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

        public (StatBreakdownDisplay Duration, StatBreakdownDisplay MoveSlowPercent, StatBreakdownDisplay AttackSlowPercent, StatBreakdownDisplay ActionSlowPercent) CalculateStats(int resonantCellCount)
        {
            return (
                new StatBreakdownDisplay(StatLabel.ChillDuration, BaseDuration.Seconds, resonantCellCount * durationPerResonance),
                new StatBreakdownDisplay(StatLabel.MovementSpeedReductionPercent, baseMoveSlowPercent, resonantCellCount * moveSlowPercentPerResonantCell),
                new StatBreakdownDisplay(StatLabel.AttackSpeedReductionPercent, baseAttackSlowPercent, resonantCellCount * attackSlowPercentPerResonantCell),
                new StatBreakdownDisplay(StatLabel.ActionRateReductionPercent, baseActionSlowPercent, resonantCellCount * actionSlowPercentPerResonantCell)
            );
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"{stats.Duration}\n{stats.MoveSlowPercent}\n{stats.AttackSlowPercent}\n{stats.ActionSlowPercent}";
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