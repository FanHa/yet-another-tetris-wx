using UnityEngine;
using UnityEngine.Serialization;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Wind Shift")]
    public class WindShiftSkillConfig : SkillConfig<WindShiftLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private WindShiftBuffDefinition windShiftBuffDefinition;

        public WindShiftBuffDefinition WindShiftBuffDefinition => windShiftBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(windShiftBuffDefinition, nameof(windShiftBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class WindShiftLevelConfig : SkillLevelConfig
    {
    }
}