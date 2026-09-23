using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Shadow/Shadow Attack")]
    public class ShadowAttackSkillConfig : SkillConfig<ShadowAttackLevelConfig>
    {
    }

    [System.Serializable]
    public class ShadowAttackLevelConfig : SkillLevelConfig
    {
        public float VulnerabilityPercent;

        public float VulnerabilityPercentPerShadowCell;

        public float DotDuration;
    }
}
