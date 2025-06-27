using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/BlazingFieldConfigGroup")]
    public class BlazingFieldConfigGroup : SkillConfigGroup
    {
        public List<BlazingFieldConfig> LevelConfigs;
    }

    [System.Serializable]
    public class BlazingFieldConfig : SkillConfig
    {
        [Header("基础属性")]
        public float BaseRadius = 1.5f;
        public float BaseDuration = 5f;
        public float BaseDotDps = 5f;
        public float BaseDotDuration = 3f;

        [Header("每个火块加成")]
        public float RadiusPerFireCell = 0.2f;
        public float DurationPerFireCell = 1f;
        public float DotDpsPerFireCell = 2f;
        public float DotDurationPerFireCell = 0.5f;

        [Header("消耗")]
        public float RequiredEnergy = 120f;
    }
}
