using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Freeze")]
    public sealed class FreezeBuffDefinition : BuffDefinition
    {
        [SerializeField] private float durationPerResonance = 0.5f;

        public AdditiveValueBreakdown CalculateStats(int resonantCellCount) =>
            new AdditiveValueBreakdown(BaseDuration.Seconds, resonantCellCount * durationPerResonance);

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