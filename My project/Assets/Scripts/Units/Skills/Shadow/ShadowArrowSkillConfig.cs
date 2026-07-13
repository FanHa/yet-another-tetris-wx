using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowArrowSkillConfig")]
    public class ShadowArrowSkillConfig : SkillConfig<ShadowArrowLevelConfig>
    {
    }

    [System.Serializable]
    public class ShadowArrowLevelConfig : SkillLevelConfig
    {
        public float RequiredEnergy ;

        public float Damage;
        public float DamagePerShadowCell;
        
        public float VulnerabilityPercent;
        public float VulnerabilityPercentPerShadowCell;
        public float VulnerabilityDuration;
    }
}
