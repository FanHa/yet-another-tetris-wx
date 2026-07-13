using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FireballSkillConfig")]
    public class FireballSkillConfig : SkillConfig<FireballLevelConfig>
    {
    }

    [System.Serializable]
    public class FireballLevelConfig : SkillLevelConfig
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
