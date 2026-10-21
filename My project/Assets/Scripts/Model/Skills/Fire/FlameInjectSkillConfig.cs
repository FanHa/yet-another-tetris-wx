using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Flame Inject")]
    public class FlameInjectSkillConfig : SkillConfig<FlameInjectLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private FlameInjectBuffDefinition flameInjectBuffDefinition;

        public FlameInjectBuffDefinition FlameInjectBuffDefinition => flameInjectBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(flameInjectBuffDefinition, nameof(flameInjectBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class FlameInjectLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float BuffDuration;
    }
}
