using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/HitAndRunConfigGroup")]
    public class HitAndRunConfigGroup : SkillConfigGroup
    {
        public List<HitAndRunConfig> LevelConfigs;
    }
    [System.Serializable]
    public class HitAndRunConfig : SkillConfig
    {
        public float BuffDuration;
    }
}