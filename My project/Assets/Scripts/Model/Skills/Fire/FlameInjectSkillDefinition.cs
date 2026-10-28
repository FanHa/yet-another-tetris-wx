using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Fire/Flame Inject")]
    public sealed class FlameInjectSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private FlameInjectBuffDefinition flameInjectBuffDefinition;

        public FlameInjectBuffDefinition FlameInjectBuffDefinition => flameInjectBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(flameInjectBuffDefinition, nameof(flameInjectBuffDefinition));
        }
#endif
    }
}