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
        [SerializeField] private float damagePerResonantCell = 5f;

        public Charge ProjectilePrefab => projectilePrefab;
        public float RequiredEnergy => requiredEnergy;
        public float ChargeDamage => chargeDamage;
        public float DamageBonusPerSpeed => damageBonusPerSpeed;
        public float DamagePerResonantCell => damagePerResonantCell;

        public AdditiveValueBreakdown CalculateDamage(float moveSpeed, int resonantCellCount) =>
            new(chargeDamage, moveSpeed * damageBonusPerSpeed + resonantCellCount * damagePerResonantCell);

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
            ValidateFiniteNonNegative(damagePerResonantCell, nameof(damagePerResonantCell));
        }
#endif
    }
}