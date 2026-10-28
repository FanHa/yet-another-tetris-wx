using UnityEngine;

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