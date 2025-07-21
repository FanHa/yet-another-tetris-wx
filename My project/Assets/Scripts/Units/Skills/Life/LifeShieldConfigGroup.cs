using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifeShieldConfigGroup")]
    public class LifeShieldConfigGroup : SkillConfigGroup
    {
        public List<LifeShieldConfig> LevelConfigs;
    }

    [System.Serializable]
    public class LifeShieldConfig : SkillConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float LifeCostPercent;
        public float AbsorbPercent;
        public float BuffDuration;

    }
}
