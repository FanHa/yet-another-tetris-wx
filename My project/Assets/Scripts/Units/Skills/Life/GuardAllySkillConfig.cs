using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/GuardAllySkillConfig")]
    public class GuardAllySkillConfig : SkillConfig<GuardAllyLevelConfig>
    {
    }

    [System.Serializable]
    public class GuardAllyLevelConfig : SkillLevelConfig
    {
    }
}
