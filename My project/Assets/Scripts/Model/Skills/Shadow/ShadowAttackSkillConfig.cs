using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Shadow/Shadow Attack")]
    public class ShadowAttackSkillConfig : SkillConfig<ShadowAttackLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private ShadowAttackBuffDefinition shadowAttackBuffDefinition;

        public ShadowAttackBuffDefinition ShadowAttackBuffDefinition => shadowAttackBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(shadowAttackBuffDefinition, nameof(shadowAttackBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class ShadowAttackLevelConfig : SkillLevelConfig
    {
    }
}
