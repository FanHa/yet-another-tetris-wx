using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifePowerSkillConfig")]
    public class LifePowerSkillConfig : SkillConfig<LifePowerLevelConfig>
    {
    }

    [System.Serializable]
    public class LifePowerLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float HealthToAtkPercent;
        public float BuffDuration;

    }
}
