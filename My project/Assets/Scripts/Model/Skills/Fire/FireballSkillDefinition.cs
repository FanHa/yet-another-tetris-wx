using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Fire/Fireball")]
    public sealed class FireballSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private Fireball projectilePrefab;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 100f;

        public Fireball ProjectilePrefab => projectilePrefab;
        public float RequiredEnergy => requiredEnergy;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}