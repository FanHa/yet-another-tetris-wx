using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Shadow/Shadow Attack")]
    public class ShadowAttackSkillConfig : SkillConfig<ShadowAttackLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition shadowAttackBuffDefinition;
        [SerializeField] private BuffDefinition vulnerabilityBuffDefinition;

        public BuffDefinition ShadowAttackBuffDefinition => shadowAttackBuffDefinition;
        public BuffDefinition VulnerabilityBuffDefinition => vulnerabilityBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(shadowAttackBuffDefinition, nameof(shadowAttackBuffDefinition));
            ValidateRequiredReference(vulnerabilityBuffDefinition, nameof(vulnerabilityBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class ShadowAttackLevelConfig : SkillLevelConfig
    {
        public float VulnerabilityPercent;

        public float VulnerabilityPercentPerShadowCell;

        public float DotDuration;
    }
}
