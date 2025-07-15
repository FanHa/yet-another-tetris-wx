using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FireballConfigGroup")]
    public class FireballConfigGroup : SkillConfigGroup
    {
        public List<FireballConfig> LevelConfigs;
    }

    [System.Serializable]
    public class FireballConfig : SkillConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float DotBaseDamage;
        public float DotDuration;

        [Header("加成")]
        public float DotAddtionPerFireCell; // 每个火系方块增加的伤害
    }
}
