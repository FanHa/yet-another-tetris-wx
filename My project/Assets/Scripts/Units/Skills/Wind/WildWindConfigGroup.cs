using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/WildWindConfigGroup")]
    public class WildWindConfigGroup : SkillConfigGroup
    {
        public List<WildWindConfig> LevelConfigs;
    }

    [System.Serializable]
    public class WildWindConfig : SkillConfig
    {
        [Header("技能消耗")]
        public float RequiredEnergy;

        [Header("基础属性")]
        public float Damage;
        public float DebuffDuration;
        public int MoveSlowPercent;
        public int AtkReducePercent;
        public float Radius;
        public float Duration;

        [Header("加成")]
        public float DamageAdditionPerWindCell = 3f; // 每个风系方块增加的伤害
    }
}