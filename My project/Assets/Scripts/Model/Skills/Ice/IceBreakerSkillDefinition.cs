using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Ice/Ice Breaker")]
    public sealed class IceBreakerSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private IceBreakerBuffDefinition iceBreakerBuffDefinition;

        public IceBreakerBuffDefinition IceBreakerBuffDefinition => iceBreakerBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(iceBreakerBuffDefinition, nameof(iceBreakerBuffDefinition));
        }
#endif
    }
}