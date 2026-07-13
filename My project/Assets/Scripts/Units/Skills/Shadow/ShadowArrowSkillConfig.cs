using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowArrowConfigGroup")]
    public class ShadowArrowConfigGroup : SkillConfigGroup
    {
        public List<ShadowArrowConfig> LevelConfigs;
    }

    [System.Serializable]
    public class ShadowArrowConfig : SkillConfig
    {
        public float RequiredEnergy ;

        public float Damage;
        public float DamagePerShadowCell;
        
        public float VulnerabilityPercent;
        public float VulnerabilityPercentPerShadowCell;
        public float VulnerabilityDuration;
    }
}
