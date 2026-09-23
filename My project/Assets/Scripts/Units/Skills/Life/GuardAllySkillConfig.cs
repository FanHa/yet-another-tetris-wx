using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Guard Ally")]
    public class GuardAllySkillConfig : SkillConfig<GuardAllyLevelConfig>
    {
    }

    [System.Serializable]
    public class GuardAllyLevelConfig : SkillLevelConfig
    {
    }
}
