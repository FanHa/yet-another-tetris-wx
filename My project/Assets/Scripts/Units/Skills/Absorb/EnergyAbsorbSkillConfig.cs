using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/EnergyAbsorbSkillConfig")]
    public class EnergyAbsorbSkillConfig : SkillConfig<EnergyAbsorbLevelConfig>
    {
    }

    [System.Serializable]
    public class EnergyAbsorbLevelConfig : SkillLevelConfig
    {
        public float BuffDuration;
        public float BaseEnergyAbsorbPerSkillCast;
        public float EnergyAbsorbPerAbsorbCell;
    }
}
