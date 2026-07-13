using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowAttackConfigGroup")]
    public class ShadowAttackConfigGroup : SkillConfigGroup
    {
        public List<ShadowAttackConfig> LevelConfigs;
    }

    [System.Serializable]
    public class ShadowAttackConfig : SkillConfig
    {
        public float VulnerabilityPercent;

        public float VulnerabilityPercentPerShadowCell;

        public float DotDuration;
    }
}
