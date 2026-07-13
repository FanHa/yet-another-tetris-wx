using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/IcyCageConfigGroup")]
    public class IcyCageConfigGroup : SkillConfigGroup
    {
        public List<IcyCageConfig> LevelConfigs;
    }

    [System.Serializable]
    public class IcyCageConfig : SkillConfig
    {
        [Header("冻结效果")]
        [Tooltip("基础冻结持续时间")]
        public float BaseFreezeDuration = 2f;
        [Tooltip("每个冰块增加的冻结持续时间")]
        public float FreezeDurationPerIceCell = 0.5f;

        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 65f;
    }
}
