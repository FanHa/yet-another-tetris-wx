using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowStepSkillConfig")]
    public class ShadowStepSkillConfig : SkillConfig<ShadowStepLevelConfig>
    {
    }

    [System.Serializable]
    public class ShadowStepLevelConfig : SkillLevelConfig
    {

        [Header("消耗")]
        public float RequiredEnergy;
        public float InitEnergy;

        public float VulnerabilityPercent;
        public float VulnerabilityPercentPerShadowCell;
        public float Damage;
        public float DamagePerShadowCell;

        public float DebuffDuration;
    }
}
