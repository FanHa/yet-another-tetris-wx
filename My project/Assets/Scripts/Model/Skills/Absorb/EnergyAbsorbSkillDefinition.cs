using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Absorb/Energy Absorb")]
    public sealed class EnergyAbsorbSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private EnergyAbsorbBuffDefinition energyAbsorbBuffDefinition;

        public EnergyAbsorbBuffDefinition EnergyAbsorbBuffDefinition => energyAbsorbBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(energyAbsorbBuffDefinition, nameof(energyAbsorbBuffDefinition));
        }
#endif
    }
}