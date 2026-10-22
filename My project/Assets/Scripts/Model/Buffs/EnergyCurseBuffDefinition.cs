using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Energy Curse")]
    public sealed class EnergyCurseBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration;
        [SerializeField] private float baseEnergyRegenReductionPercent;
        [SerializeField] private float energyRegenReductionPerResonantCell;
        [SerializeField] private float maxEnergyRegenReductionPercent;

        public (StatValue Duration, StatValue EnergyRegenReductionPercent) CalculateStats(int resonantCellCount)
        {
            float bonus = Mathf.Min(
                resonantCellCount * energyRegenReductionPerResonantCell,
                maxEnergyRegenReductionPercent - baseEnergyRegenReductionPercent);

            return (
                new StatValue("诅咒持续", duration),
                new StatValue("能量回复降低(%)", baseEnergyRegenReductionPercent, bonus)
            );
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"{Description}\n{stats.Duration}\n{stats.EnergyRegenReductionPercent}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (duration <= 0f || float.IsNaN(duration) || float.IsInfinity(duration))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be finite and greater than zero.", this);
            }

            ValidatePercent(baseEnergyRegenReductionPercent, nameof(baseEnergyRegenReductionPercent));
            ValidatePercent(maxEnergyRegenReductionPercent, nameof(maxEnergyRegenReductionPercent));
            if (energyRegenReductionPerResonantCell < 0f || float.IsNaN(energyRegenReductionPerResonantCell) || float.IsInfinity(energyRegenReductionPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(energyRegenReductionPerResonantCell)} must be finite and non-negative.", this);
            }

            if (baseEnergyRegenReductionPercent > maxEnergyRegenReductionPercent)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(baseEnergyRegenReductionPercent)} must not exceed {nameof(maxEnergyRegenReductionPercent)}.", this);
            }
        }

        private void ValidatePercent(float value, string fieldName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 80f)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and between 0 and 80.", this);
            }
        }
#endif
    }
}