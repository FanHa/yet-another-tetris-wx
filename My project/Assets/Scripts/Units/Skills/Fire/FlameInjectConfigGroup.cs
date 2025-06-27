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
        [Header("火焰伤害")]
        public float BaseFireDamage = 1f;
        public float FireCellDamageBonus = 1f;

        [Header("灼烧效果")]
        public float BaseDotDps = 1f;
        public float DotDpsPerFireCell = 1f;
        public float BaseDotDuration = 2f;
        public float DotDurationPerFireCell = 1f;

        [Header("Buff")]
        public float BuffDuration = -1f;
    }
}
