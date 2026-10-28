using UnityEngine;
using Model;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Fortitude")]
    public sealed class LifeFortitudeBuffDefinition : BuffDefinition
    {
        [SerializeField, Min(0f)] private float maxHealthIncreasePerResonantCell = 5f;

        public AdditiveValueBreakdown CalculateStats(float baseIncrease, int resonantCellCount) =>
            new(baseIncrease, resonantCellCount * maxHealthIncreasePerResonantCell);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (maxHealthIncreasePerResonantCell < 0f || float.IsNaN(maxHealthIncreasePerResonantCell) || float.IsInfinity(maxHealthIncreasePerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(maxHealthIncreasePerResonantCell)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}