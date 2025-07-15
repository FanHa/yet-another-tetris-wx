using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FlameInjectConfigGroup")]
    public class FlameInjectConfigGroup : SkillConfigGroup
    {
        public List<FlameInjectConfig> LevelConfigs;
    }

    [System.Serializable]
    public class FlameInjectConfig : SkillConfig
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
