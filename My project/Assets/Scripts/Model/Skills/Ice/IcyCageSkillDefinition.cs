using Model.Buffs;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Ice/Icy Cage")]
    public sealed class IcyCageSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private IcyCage projectilePrefab;
        [Header("Buff")]
        [SerializeField] private FreezeBuffDefinition freezeBuffDefinition;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 80f;

        public IcyCage ProjectilePrefab => projectilePrefab;
        public FreezeBuffDefinition FreezeBuffDefinition => freezeBuffDefinition;
        public float RequiredEnergy => requiredEnergy;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(freezeBuffDefinition, nameof(freezeBuffDefinition));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}