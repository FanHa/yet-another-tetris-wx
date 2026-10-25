using Model;
using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Power")]
    public sealed class LifePowerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float healthToAtkPercent;
        [SerializeField] private float healthToAtkPercentPerResonantCell;

        public AdditiveValueBreakdown CalculateCasterMaxHealthConversionPercent(int lifeResonanceCount) =>
            new(healthToAtkPercent, lifeResonanceCount * healthToAtkPercentPerResonantCell);

        public float CalculateAttackHitBonusDamage(float casterMaxHealthAtCast, float maxHealthConversionPercent) =>
            casterMaxHealthAtCast * (maxHealthConversionPercent / 100f);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (healthToAtkPercent < 0f || float.IsNaN(healthToAtkPercent) || float.IsInfinity(healthToAtkPercent))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(healthToAtkPercent)} must be finite and non-negative.", this);
            }

            if (healthToAtkPercentPerResonantCell < 0f || float.IsNaN(healthToAtkPercentPerResonantCell) || float.IsInfinity(healthToAtkPercentPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(healthToAtkPercentPerResonantCell)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}