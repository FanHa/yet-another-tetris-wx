using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Life/Life Fortitude")]
    public sealed class LifeFortitudeSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private LifeFortitudeBuffDefinition buffDefinition;
        [Header("Health")]
        [SerializeField, Min(0.01f)] private float maxHealthIncrease = 10f;

        public LifeFortitudeBuffDefinition BuffDefinition => buffDefinition;
        public float MaxHealthIncrease => maxHealthIncrease;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(buffDefinition, nameof(buffDefinition));
            ValidateFinitePositive(maxHealthIncrease, nameof(maxHealthIncrease));
        }
#endif
    }
}