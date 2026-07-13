using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/AttackBoostConfigGroup")]
    public class AttackBoostConfigGroup : SkillConfigGroup
    {
        public List<AttackBoostConfig> LevelConfigs;
    }
    [System.Serializable]
    public class AttackBoostConfig : SkillConfig
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