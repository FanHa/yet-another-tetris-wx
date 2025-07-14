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
        public float AttackRangeBonus;
        [Tooltip("造成伤害降低百分比（如30表示降低30%）")]
        public float DamageReducePercent;
        [Tooltip("受到伤害提升百分比（如30表示提升30%）")]
        public float TakeDamageIncreasePercent;

    }
}