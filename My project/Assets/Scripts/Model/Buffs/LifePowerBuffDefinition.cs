using UnityEngine;
using Model;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Power")]
    public sealed class LifePowerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float healthToAtkPercent;
        [SerializeField] private float healthToAtkPercentPerResonantCell;

        public (AdditiveValueBreakdown ConversionPercent, float AttackHitBonusDamage) CalculateStats(float casterMaxHealthAtCast, int resonanceCount)
        {
            var conversionPercent = new AdditiveValueBreakdown(
                healthToAtkPercent,
                resonanceCount * healthToAtkPercentPerResonantCell);
            float attackHitBonusDamage = casterMaxHealthAtCast * (conversionPercent.FinalValue / 100f);
            return (conversionPercent, attackHitBonusDamage);
        }

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