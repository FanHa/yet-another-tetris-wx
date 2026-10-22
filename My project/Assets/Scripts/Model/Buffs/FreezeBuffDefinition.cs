using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Freeze")]
    public sealed class FreezeBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseDuration = 2f;
        [SerializeField] private float durationPerResonantCell = 0.5f;

        public StatValue CalculateStats(int resonantCellCount) =>
            new StatValue("冻结持续时间", baseDuration, resonantCellCount * durationPerResonantCell);

        public string DescribeEffect(int resonantCellCount) =>
            $"完全冻结,无法行动,能量回复为0\n{CalculateStats(resonantCellCount)}";

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(DisplayName)} is not assigned.", this);
            }

            if (baseDuration <= 0f || float.IsNaN(baseDuration) || float.IsInfinity(baseDuration))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(baseDuration)} must be finite and greater than zero.", this);
            }

            if (durationPerResonantCell < 0f || float.IsNaN(durationPerResonantCell) || float.IsInfinity(durationPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(durationPerResonantCell)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}