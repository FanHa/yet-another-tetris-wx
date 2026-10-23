using Units.Skills;
using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Fortitude")]
    public sealed class LifeFortitudeBuffDefinition : BuffDefinition
    {
        [SerializeField, Min(0f)] private float maxHealthIncreasePerResonantCell = 5f;

        public float CalculateMaxHealthIncrease(float baseIncrease, int resonantCellCount) =>
            baseIncrease + resonantCellCount * maxHealthIncreasePerResonantCell;

        public string DescribeEffect(float baseIncrease, int resonantCellCount) =>
            new StatValue("最大生命", baseIncrease, resonantCellCount * maxHealthIncreasePerResonantCell).ToString();

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