using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Life/Life Echo")]
    public sealed class LifeEchoSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private LifeEchoBuffDefinition lifeEchoBuffDefinition;

        public LifeEchoBuffDefinition LifeEchoBuffDefinition => lifeEchoBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(lifeEchoBuffDefinition, nameof(lifeEchoBuffDefinition));
        }
#endif
    }
}