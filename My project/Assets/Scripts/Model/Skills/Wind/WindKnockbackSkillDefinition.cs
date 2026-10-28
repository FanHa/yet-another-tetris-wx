using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Wind/Wind Knockback")]
    public sealed class WindKnockbackSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private WindKnockbackBuffDefinition windKnockbackBuffDefinition;

        public WindKnockbackBuffDefinition WindKnockbackBuffDefinition => windKnockbackBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(windKnockbackBuffDefinition, nameof(windKnockbackBuffDefinition));
        }
#endif
    }
}