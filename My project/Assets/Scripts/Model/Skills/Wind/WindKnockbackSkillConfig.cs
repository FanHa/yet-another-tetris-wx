using UnityEngine;
using UnityEngine.Serialization;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Wind Knockback")]
    public class WindKnockbackSkillConfig : SkillConfig<WindKnockbackLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private WindKnockbackBuffDefinition windKnockbackBuffDefinition;

        public WindKnockbackBuffDefinition WindKnockbackBuffDefinition => windKnockbackBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(windKnockbackBuffDefinition, nameof(windKnockbackBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class WindKnockbackLevelConfig : SkillLevelConfig
    {
    }
}
