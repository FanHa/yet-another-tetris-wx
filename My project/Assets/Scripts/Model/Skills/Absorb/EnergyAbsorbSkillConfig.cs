using UnityEngine;
using UnityEngine.Serialization;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Absorb/Energy Absorb")]
    public class EnergyAbsorbSkillConfig : SkillConfig<EnergyAbsorbLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private EnergyAbsorbBuffDefinition energyAbsorbBuffDefinition;

        public EnergyAbsorbBuffDefinition EnergyAbsorbBuffDefinition => energyAbsorbBuffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(energyAbsorbBuffDefinition, nameof(energyAbsorbBuffDefinition));
        }
#endif
    }

    [System.Serializable]
    public class EnergyAbsorbLevelConfig : SkillLevelConfig
    {
    }
}
