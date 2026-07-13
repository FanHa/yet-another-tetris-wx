using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FlameInjectSkillConfig")]
    public class FlameInjectSkillConfig : SkillConfig<FlameInjectLevelConfig>
    {
    }

    [System.Serializable]
    public class FlameInjectLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float BuffDuration;

        [Header("属性")]
        public float BaseDotDps;
        public float DotDuration;

        [Header("加成")]
        public float DotDpsPerFireCell;

    }
}
