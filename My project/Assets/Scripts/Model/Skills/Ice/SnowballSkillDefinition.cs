using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Ice/Snowball")]
    public sealed class SnowballSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private Snowball projectilePrefab;
        [Header("Damage")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float iceCellDamageBonus = 4f;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 40f;

        public Snowball ProjectilePrefab => projectilePrefab;
        public float BaseDamage => baseDamage;
        public float IceCellDamageBonus => iceCellDamageBonus;
        public float RequiredEnergy => requiredEnergy;

        public AdditiveValueBreakdown CalculateDamage(int iceCellCount) =>
            new(baseDamage, iceCellCount * iceCellDamageBonus);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFiniteNonNegative(baseDamage, nameof(baseDamage));
            ValidateFiniteNonNegative(iceCellDamageBonus, nameof(iceCellDamageBonus));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}