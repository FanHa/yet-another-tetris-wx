using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Energy Absorb")]
    public sealed class EnergyAbsorbBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseEnergyAbsorbPerSkillCast;
        [SerializeField] private float energyAbsorbPerResonantCell;

        public AdditiveValueBreakdown CalculateStats(int resonantCellCount) =>
            new(baseEnergyAbsorbPerSkillCast, resonantCellCount * energyAbsorbPerResonantCell);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateFiniteNonNegative(baseEnergyAbsorbPerSkillCast, nameof(baseEnergyAbsorbPerSkillCast));
            ValidateFiniteNonNegative(energyAbsorbPerResonantCell, nameof(energyAbsorbPerResonantCell));
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