using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifePowerConfigGroup")]
    public class LifePowerConfigGroup : SkillConfigGroup
    {
        public List<LifePowerConfig> LevelConfigs;
    }

    [System.Serializable]
    public class LifePowerConfig : SkillConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float HealthToAtkPercent;
        public float BuffDuration;

    }
}
