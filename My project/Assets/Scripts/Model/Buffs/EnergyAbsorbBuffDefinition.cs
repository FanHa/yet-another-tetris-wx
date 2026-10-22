using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Energy Absorb")]
    public sealed class EnergyAbsorbBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration = -1f;
        [SerializeField] private float baseEnergyAbsorbPerSkillCast;
        [SerializeField] private float energyAbsorbPerResonantCell;

        public float Duration => duration;

        public (StatValue EnergyAbsorbPerSkillCast, StatValue Duration) CalculateStats(int resonantCellCount) =>
            (
                new StatValue("每次敌方施法为每个友军补充能量", baseEnergyAbsorbPerSkillCast, resonantCellCount * energyAbsorbPerResonantCell),
                new StatValue("效果持续时间", duration)
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
            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }

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