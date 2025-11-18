using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/EnergyAbsorbConfigGroup")]
    public class EnergyAbsorbConfigGroup : SkillConfigGroup
    {
        public List<EnergyAbsorbConfig> LevelConfigs;
    }

    [System.Serializable]
    public class EnergyAbsorbConfig: SkillConfig
    {
        public float BuffDuration;
        public float BaseEnergyAbsorbPerSkillCast;
        public float EnergyAbsorbPerAbsorbCell;
    }
}
