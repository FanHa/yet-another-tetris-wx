using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Power")]
    public class LifePowerSkillConfig : SkillConfig<LifePowerLevelConfig>
    {
    }

    [System.Serializable]
    public class LifePowerLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("通用")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;

        [Header("属性")]
        public float HealthToAtkPercent;
        public float BuffDuration;

    }
}
