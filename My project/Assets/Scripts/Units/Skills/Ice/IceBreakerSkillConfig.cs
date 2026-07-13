using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/IceBreakerSkillConfig")]
    public class IceBreakerSkillConfig : SkillConfig<IceBreakerLevelConfig>
    {
    }

    [System.Serializable]
    public class IceBreakerLevelConfig : SkillLevelConfig
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
