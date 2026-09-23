using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Shield")]
    public class LifeShieldSkillConfig : SkillConfig<LifeShieldLevelConfig>
    {
    }

    [System.Serializable]
    public class LifeShieldLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float LifeCostPercent;
        public float AbsorbPercent;
        public float BuffDuration;

    }
}
