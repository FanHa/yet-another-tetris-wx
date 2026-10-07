using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Wind Knockback")]
    public class WindKnockbackSkillConfig : SkillConfig<WindKnockbackLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition windKnockbackBuffDefinition;

        public BuffDefinition WindKnockbackBuffDefinition => windKnockbackBuffDefinition;

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
        [Header("属性")]
        public float BaseKnockbackDistance = 0.12f;
        public float KnockbackDistancePerWindCell = 0.01f;
        public float MaxKnockbackDistance = 0.18f;
    }
}
