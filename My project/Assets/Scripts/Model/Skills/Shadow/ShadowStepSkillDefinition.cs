using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Shadow/Shadow Step")]
    public sealed class ShadowStepSkillDefinition : SkillDefinition
    {
        [Header("Buff")]
        [SerializeField] private VulnerabilityBuffDefinition vulnerabilityBuffDefinition;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 100f;
        [SerializeField] private float initialEnergy = 50f;
        [Header("Damage")]
        [SerializeField] private float damage = 20f;
        [SerializeField] private float damagePerResonantCell;

        public VulnerabilityBuffDefinition VulnerabilityBuffDefinition => vulnerabilityBuffDefinition;
        public float RequiredEnergy => requiredEnergy;
        public float InitialEnergy => initialEnergy;
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

            ValidateRequiredReference(vulnerabilityBuffDefinition, nameof(vulnerabilityBuffDefinition));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
            ValidateFiniteNonNegative(initialEnergy, nameof(initialEnergy));
            if (initialEnergy > requiredEnergy)
            {
                Debug.LogError($"{nameof(initialEnergy)} must be between 0 and {nameof(requiredEnergy)}.", this);
            }

            ValidateFiniteNonNegative(damage, nameof(damage));
            ValidateFiniteNonNegative(damagePerResonantCell, nameof(damagePerResonantCell));
        }
#endif
    }
}