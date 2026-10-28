using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Life/Life Power")]
    public sealed class LifePowerSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private LifePowerBuffDefinition lifePowerBuffDefinition;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 60f;

        public LifePowerBuffDefinition LifePowerBuffDefinition => lifePowerBuffDefinition;
        public float RequiredEnergy => requiredEnergy;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(lifePowerBuffDefinition, nameof(lifePowerBuffDefinition));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}