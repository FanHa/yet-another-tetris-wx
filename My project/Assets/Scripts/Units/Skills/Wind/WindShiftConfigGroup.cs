using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/WindShiftConfigGroup")]
    public class WindShiftConfigGroup : SkillConfigGroup
    {
        public List<WindShiftConfig> LevelConfigs;
    }

    [System.Serializable]
    public class WindShiftConfig : SkillConfig
    {
        [Header("风形态属性加成")]
        public float AttackRangeBonus = 2f;
        public float DamageMultiplier = 0.7f;
        public float TakeDamageMultiplier = 1.3f;

        [Header("消耗")]
        public float RequiredEnergy = 40f;
    }
}