using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Shadow/Shadow Attack")]
    public sealed class ShadowAttackSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private ShadowAttackBuffDefinition shadowAttackBuffDefinition;

        public ShadowAttackBuffDefinition ShadowAttackBuffDefinition => shadowAttackBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(shadowAttackBuffDefinition, nameof(shadowAttackBuffDefinition));
        }
#endif
    }
}