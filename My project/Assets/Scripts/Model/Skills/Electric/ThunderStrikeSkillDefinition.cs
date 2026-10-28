using Model;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Electric/Thunder Strike")]
    public sealed class ThunderStrikeSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private ThunderStrikeProjectile projectilePrefab;
        [Header("Damage")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerResonantCell = 15f;
        [Header("Energy disruption")]
        [SerializeField] private float baseEnergyDisruptionAmount = 10f;
        [SerializeField] private float energyDisruptionAmountPerResonantCell = 15f;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 60f;

        public ThunderStrikeProjectile ProjectilePrefab => projectilePrefab;
        public float BaseDamage => baseDamage;
        public float DamagePerResonantCell => damagePerResonantCell;
        public float BaseEnergyDisruptionAmount => baseEnergyDisruptionAmount;
        public float EnergyDisruptionAmountPerResonantCell => energyDisruptionAmountPerResonantCell;
        public float RequiredEnergy => requiredEnergy;

        public (AdditiveValueBreakdown Damage, float EnergyDisruptionAmount) CalculateStats(int resonantCellCount)
        {
            return (
                new AdditiveValueBreakdown(baseDamage, resonantCellCount * damagePerResonantCell),
                baseEnergyDisruptionAmount + resonantCellCount * energyDisruptionAmountPerResonantCell);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateFiniteNonNegative(baseDamage, nameof(baseDamage));
            ValidateFiniteNonNegative(damagePerResonantCell, nameof(damagePerResonantCell));
            ValidateFiniteNonNegative(baseEnergyDisruptionAmount, nameof(baseEnergyDisruptionAmount));
            ValidateFiniteNonNegative(energyDisruptionAmountPerResonantCell, nameof(energyDisruptionAmountPerResonantCell));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}