using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Shadow/Shadow Arrow")]
    public sealed class ShadowArrowSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private ShadowArrow projectilePrefab;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 60f;
        [Header("Damage")]
        [SerializeField] private float damage = 20f;
        [SerializeField] private float damagePerResonantCell;

        public ShadowArrow ProjectilePrefab => projectilePrefab;
        public float RequiredEnergy => requiredEnergy;
        public float Damage => damage;
        public float DamagePerResonantCell => damagePerResonantCell;

        public AdditiveValueBreakdown CalculateDamage(int resonantCellCount) =>
            new(damage, resonantCellCount * damagePerResonantCell);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
            ValidateFiniteNonNegative(damage, nameof(damage));
            ValidateFiniteNonNegative(damagePerResonantCell, nameof(damagePerResonantCell));
        }
#endif
    }
}