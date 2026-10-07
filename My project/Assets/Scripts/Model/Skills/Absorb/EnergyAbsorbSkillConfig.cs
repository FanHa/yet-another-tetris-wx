using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Absorb/Energy Absorb")]
    public class EnergyAbsorbSkillConfig : SkillConfig<EnergyAbsorbLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition energyAbsorbBuffDefinition;

        public BuffDefinition EnergyAbsorbBuffDefinition => energyAbsorbBuffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(energyAbsorbBuffDefinition, nameof(energyAbsorbBuffDefinition));
            foreach (var (i, level) in GetValidLevelConfigs())
            {
                ValidateFiniteNonNegative(level.BaseEnergyAbsorbPerSkillCast, $"{nameof(LevelConfigs)}[{i}].{nameof(level.BaseEnergyAbsorbPerSkillCast)}");
                ValidateFiniteNonNegative(level.EnergyAbsorbPerAbsorbCell, $"{nameof(LevelConfigs)}[{i}].{nameof(level.EnergyAbsorbPerAbsorbCell)}");
            }
        }
#endif
    }

    [System.Serializable]
    public class EnergyAbsorbLevelConfig : SkillLevelConfig
    {
        public float BuffDuration;
        public float BaseEnergyAbsorbPerSkillCast;
        public float EnergyAbsorbPerAbsorbCell;
    }
}
