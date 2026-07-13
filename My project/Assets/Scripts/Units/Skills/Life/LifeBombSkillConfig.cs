using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifeBombSkillConfig")]
    public class LifeBombSkillConfig : SkillConfig<LifeBombLevelConfig>
    {
    }

    [System.Serializable]
    public class LifeBombLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float HealthCostPercent;


    }
}
