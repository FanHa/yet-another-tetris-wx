using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/WindShiftSkillConfig")]
    public class WindShiftSkillConfig : SkillConfig<WindShiftLevelConfig>
    {
    }

    [System.Serializable]
    public class WindShiftLevelConfig : SkillLevelConfig
    {

        public float AttackRangeBonus;
        public float AttackRangePerWindCell;
    }
}