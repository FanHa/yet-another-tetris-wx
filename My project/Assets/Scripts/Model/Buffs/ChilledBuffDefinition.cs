using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Chilled")]
    public sealed class ChilledBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseDuration;
        [SerializeField] private float durationPerResonantCell;
        [SerializeField] private int baseMoveSlowPercent;
        [SerializeField] private int moveSlowPercentPerResonantCell;
        [SerializeField] private int baseAttackSlowPercent;
        [SerializeField] private int attackSlowPercentPerResonantCell;
        [SerializeField] private int baseActionSlowPercent;
        [SerializeField] private int actionSlowPercentPerResonantCell;

        public (StatValue Duration, StatValue MoveSlowPercent, StatValue AttackSlowPercent, StatValue ActionSlowPercent) CalculateStats(int resonantCellCount)
        {
            return (
                new StatValue("减速效果持续时间", baseDuration, resonantCellCount * durationPerResonantCell),
                new StatValue("移动速度降低(%)", baseMoveSlowPercent, resonantCellCount * moveSlowPercentPerResonantCell),
                new StatValue("攻击速度降低(%)", baseAttackSlowPercent, resonantCellCount * attackSlowPercentPerResonantCell),
                new StatValue("动作速率降低(%)", baseActionSlowPercent, resonantCellCount * actionSlowPercentPerResonantCell)
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
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(DisplayName)} is not assigned.", this);
            }

            ValidateFinitePositive(baseDuration, nameof(baseDuration));
            ValidateFiniteNonNegative(durationPerResonantCell, nameof(durationPerResonantCell));
            ValidateNonNegative(baseMoveSlowPercent, nameof(baseMoveSlowPercent));
            ValidateNonNegative(moveSlowPercentPerResonantCell, nameof(moveSlowPercentPerResonantCell));
            ValidateNonNegative(baseAttackSlowPercent, nameof(baseAttackSlowPercent));
            ValidateNonNegative(attackSlowPercentPerResonantCell, nameof(attackSlowPercentPerResonantCell));
            ValidateNonNegative(baseActionSlowPercent, nameof(baseActionSlowPercent));
            ValidateNonNegative(actionSlowPercentPerResonantCell, nameof(actionSlowPercentPerResonantCell));
        }

        private void ValidateFinitePositive(float value, string fieldName)
        {
            if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and greater than zero.", this);
            }
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