using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/AttackBoostSkillConfig")]
    public class AttackBoostSkillConfig : SkillConfig<AttackBoostLevelConfig>
    {
    }
    [System.Serializable]
    public class AttackBoostLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;
        public float Duration;

        [Header("属性")]
        public float AtkSpeedPercent;

        [Header("加成")]
        public float AtkSpeedAdditionPercentPerWindCell; // 每个风系方块增加的攻击速度百分比
    }
}