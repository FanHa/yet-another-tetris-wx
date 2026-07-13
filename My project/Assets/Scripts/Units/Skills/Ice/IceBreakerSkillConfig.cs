using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/IceBreakerConfigGroup")]
    public class IceBreakerConfigGroup : SkillConfigGroup
    {
        public List<IceBreakerConfig> LevelConfigs;
    }

    [System.Serializable]
    public class IceBreakerConfig : SkillConfig
    {
        [Header("通用")]
        public float BuffDuration;

        [Header("属性")]
        public float BaseExtraDamage;
        public float MultiplierByChilledLayer;

        [Header("加成")]
        public float ExtraDamagePerIceCell;

    }
}
