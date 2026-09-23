using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Wind Shift")]
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