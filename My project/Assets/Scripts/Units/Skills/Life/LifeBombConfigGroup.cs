using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifeBombConfigGroup")]
    public class LifeBombConfigGroup : SkillConfigGroup
    {
        public List<LifeBombConfig> LevelConfigs;
    }

    [System.Serializable]
    public class LifeBombConfig : SkillConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float HealthCostPercent;


    }
}
