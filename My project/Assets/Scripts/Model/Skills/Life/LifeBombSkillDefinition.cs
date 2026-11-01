using Units.Projectiles;
using UnityEngine;
using Model;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Life/Life Bomb")]
    public sealed class LifeBombSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private LifeBomb projectilePrefab;
        [SerializeField] private GameObject temporaryTargetPrefab;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 80f;
        [SerializeField] private float healthCostPercent = 3f;
        [Header("Damage")]
        [SerializeField] private float damagePercentPerResonantCell = 3f;

        public LifeBomb ProjectilePrefab => projectilePrefab;
        public GameObject TemporaryTargetPrefab => temporaryTargetPrefab;
        public float RequiredEnergy => requiredEnergy;
        public float HealthCostPercent => healthCostPercent;
        public float DamagePercentPerResonantCell => damagePercentPerResonantCell;

        public (AdditiveValueBreakdown HealthCostPercent, AdditiveValueBreakdown Damage, float HealthCost) CalculateStats(
            float currentHealth,
            int resonantCellCount)
        {
            var healthCostPercentStats = new AdditiveValueBreakdown(healthCostPercent);
            float healthCost = Mathf.Clamp(currentHealth * (healthCostPercentStats.FinalValue / 100f), 1f, currentHealth);
            var damage = new AdditiveValueBreakdown(
                healthCost,
                currentHealth * resonantCellCount * damagePercentPerResonantCell / 100f);
            return (healthCostPercentStats, damage, healthCost);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(temporaryTargetPrefab, nameof(temporaryTargetPrefab));
            if (temporaryTargetPrefab != null && temporaryTargetPrefab.GetComponent<Collider2D>() == null)
            {
                Debug.LogError($"{nameof(temporaryTargetPrefab)} must have a {nameof(Collider2D)} component.", this);
            }

            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
            ValidateFiniteNonNegative(healthCostPercent, nameof(healthCostPercent));
            ValidateFiniteNonNegative(damagePercentPerResonantCell, nameof(damagePercentPerResonantCell));
        }
#endif
    }
}