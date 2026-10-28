using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Wild Wind Debuff")]
    public sealed class WildWindDebuffDefinition : BuffDefinition
    {
        [SerializeField] private float durationPerResonance;
        [SerializeField] private int baseMoveSlowPercent;
        [SerializeField] private int moveSlowPercentPerResonantCell;
        [SerializeField] private int baseAtkReducePercent;
        [SerializeField] private int atkReducePercentPerResonantCell;

        public (BuffDuration Duration, AdditiveValueBreakdown MoveSlowPercent, AdditiveValueBreakdown AtkReducePercent) CalculateStats(int resonantCellCount) =>
            (
                BuffDuration.ForSeconds(BaseDuration.Seconds + resonantCellCount * durationPerResonance),
                new AdditiveValueBreakdown(baseMoveSlowPercent, resonantCellCount * moveSlowPercentPerResonantCell),
                new AdditiveValueBreakdown(baseAtkReducePercent, resonantCellCount * atkReducePercentPerResonantCell)
            );

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateTimedBaseDuration();

            if (durationPerResonance < 0f || float.IsNaN(durationPerResonance) || float.IsInfinity(durationPerResonance))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(durationPerResonance)} must be finite and non-negative.", this);
            }

            ValidateNonNegative(baseMoveSlowPercent, nameof(baseMoveSlowPercent));
            ValidateNonNegative(moveSlowPercentPerResonantCell, nameof(moveSlowPercentPerResonantCell));
            ValidateNonNegative(baseAtkReducePercent, nameof(baseAtkReducePercent));
            ValidateNonNegative(atkReducePercentPerResonantCell, nameof(atkReducePercentPerResonantCell));
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