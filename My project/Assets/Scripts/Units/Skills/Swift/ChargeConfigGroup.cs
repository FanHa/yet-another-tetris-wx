using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ChargeConfigGroup")]
    public class ChargeConfigGroup : SkillConfigGroup
    {
        public List<ChargeConfig> LevelConfigs;
    }
    [System.Serializable]
    public class ChargeConfig : SkillConfig
    {
        public float RequiredEnergy;

        public float ChargeDamage;

        public float DamageBonusPerSpeed;

    }
}