using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Energy Absorb")]
    public sealed class EnergyAbsorbBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseEnergyAbsorbPerSkillCast;
        [SerializeField] private float energyAbsorbPerResonantCell;

        public (StatBreakdownDisplay EnergyAbsorbPerSkillCast, string Duration) CalculateStats(int resonantCellCount) =>
            (
                new StatBreakdownDisplay(StatLabel.EnergyPerEnemySkillCast, baseEnergyAbsorbPerSkillCast, resonantCellCount * energyAbsorbPerResonantCell),
                BaseDuration.ToDisplayString("效果持续时间")
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"敌对单位施放技能时，为己方除自身外的每个活动单位补充能量。\n{stats.EnergyAbsorbPerSkillCast}\n{stats.Duration}";
        }

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