using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Swift/Charge")]
    public sealed class ChargeSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private Charge projectilePrefab;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 60f;
        [Header("Damage")]
        [SerializeField] private float chargeDamage = 20f;
        [SerializeField] private float damageBonusPerSpeed = 2f;
        [Header("Movement")]
        [SerializeField] private int avoidancePriority;

        public Charge ProjectilePrefab => projectilePrefab;
        public float RequiredEnergy => requiredEnergy;
        public float ChargeDamage => chargeDamage;
        public float DamageBonusPerSpeed => damageBonusPerSpeed;
        public int AvoidancePriority => avoidancePriority;

        public AdditiveValueBreakdown CalculateDamage(float moveSpeed) =>
            new(chargeDamage, moveSpeed * damageBonusPerSpeed);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
            ValidateFiniteNonNegative(chargeDamage, nameof(chargeDamage));
            ValidateFiniteNonNegative(damageBonusPerSpeed, nameof(damageBonusPerSpeed));
        }
#endif
    }
}