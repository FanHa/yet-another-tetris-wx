using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Freeze")]
    public sealed class FreezeBuffDefinition : BuffDefinition
    {
        [SerializeField] private float durationPerResonance = 0.5f;

        public StatBreakdownDisplay CalculateStats(int resonantCellCount) =>
            new StatBreakdownDisplay(StatLabel.FreezeDuration, BaseDuration.Seconds, resonantCellCount * durationPerResonance);

        public string DescribeEffect(int resonantCellCount) =>
            $"完全冻结,无法行动,能量回复为0\n{CalculateStats(resonantCellCount)}";

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateTimedBaseDuration();

            if (durationPerResonance < 0f || float.IsNaN(durationPerResonance) || float.IsInfinity(durationPerResonance))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(durationPerResonance)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}