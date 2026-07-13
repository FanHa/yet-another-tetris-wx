using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowAttackSkillConfig")]
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
