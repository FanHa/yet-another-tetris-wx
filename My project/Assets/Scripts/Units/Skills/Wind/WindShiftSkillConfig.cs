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

        public float AttackRangeBonus;
        public float AttackRangePerWindCell;
    }
}