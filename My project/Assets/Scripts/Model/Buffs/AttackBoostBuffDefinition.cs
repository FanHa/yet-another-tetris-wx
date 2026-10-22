using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Attack Boost")]
    public sealed class AttackBoostBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseAtkSpeedPercent;
        [SerializeField] private float atkSpeedPercentPerResonantCell;

        public (string Duration, StatValue AtkSpeedPercent) CalculateStats(int resonantCellCount) =>
            (
                BaseDuration.ToDisplayString("持续时间"),
                new StatValue("攻击速度提升(%)", baseAtkSpeedPercent, resonantCellCount * atkSpeedPercentPerResonantCell)
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"{stats.Duration}\n{stats.AtkSpeedPercent}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateTimedBaseDuration();

            ValidateFiniteNonNegative(baseAtkSpeedPercent, nameof(baseAtkSpeedPercent));
            ValidateFiniteNonNegative(atkSpeedPercentPerResonantCell, nameof(atkSpeedPercentPerResonantCell));
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