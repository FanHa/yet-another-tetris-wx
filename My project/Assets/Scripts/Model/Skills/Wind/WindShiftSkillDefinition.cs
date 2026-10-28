using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Wind/Wind Shift")]
    public sealed class WindShiftSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private WindShiftBuffDefinition windShiftBuffDefinition;

        public WindShiftBuffDefinition WindShiftBuffDefinition => windShiftBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(windShiftBuffDefinition, nameof(windShiftBuffDefinition));
        }
#endif
    }
}