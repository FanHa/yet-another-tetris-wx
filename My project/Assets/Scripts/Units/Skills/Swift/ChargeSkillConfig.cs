using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ChargeSkillConfig")]
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