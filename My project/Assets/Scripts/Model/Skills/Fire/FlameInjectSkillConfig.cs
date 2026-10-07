using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Flame Inject")]
    public class FlameInjectSkillConfig : SkillConfig<FlameInjectLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition flameInjectBuffDefinition;
        [SerializeField] private BuffDefinition burnBuffDefinition;

        public BuffDefinition FlameInjectBuffDefinition => flameInjectBuffDefinition;
        public BuffDefinition BurnBuffDefinition => burnBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(flameInjectBuffDefinition, nameof(flameInjectBuffDefinition));
            ValidateRequiredReference(burnBuffDefinition, nameof(burnBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class FlameInjectLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float BuffDuration;

        [Header("属性")]
        public float BaseDotDps;
        public float DotDuration;

        [Header("加成")]
        public float DotDpsPerFireCell;

    }
}
