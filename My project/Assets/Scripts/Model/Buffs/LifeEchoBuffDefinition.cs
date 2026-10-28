using UnityEngine;
using Model;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Echo")]
    public sealed class LifeEchoBuffDefinition : BuffDefinition
    {
        [SerializeField] private float reflectPercent;
        [SerializeField] private float reflectPercentPerResonantCell;

        public AdditiveValueBreakdown CalculateStats(int resonantCellCount) =>
            new(reflectPercent, resonantCellCount * reflectPercentPerResonantCell);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (reflectPercent < 0f || float.IsNaN(reflectPercent) || float.IsInfinity(reflectPercent))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(reflectPercent)} must be finite and non-negative.", this);
            }

            if (reflectPercentPerResonantCell < 0f || float.IsNaN(reflectPercentPerResonantCell) || float.IsInfinity(reflectPercentPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(reflectPercentPerResonantCell)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}