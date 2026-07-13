using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowStepConfigGroup")]
    public class ShadowStepConfigGroup : SkillConfigGroup
    {
        public List<ShadowStepConfig> LevelConfigs;
    }

    [System.Serializable]
    public class ShadowStepConfig : SkillConfig
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
