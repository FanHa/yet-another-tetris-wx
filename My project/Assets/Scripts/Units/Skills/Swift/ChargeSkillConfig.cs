using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Swift/Charge")]
    public class ChargeSkillConfig : SkillConfig<ChargeLevelConfig>
    {
    }
    [System.Serializable]
    public class ChargeLevelConfig : SkillLevelConfig
    {
        public float RequiredEnergy;

        public float ChargeDamage;

        public float DamageBonusPerSpeed;

        public int AvoidancePriority = 0;

    }
}