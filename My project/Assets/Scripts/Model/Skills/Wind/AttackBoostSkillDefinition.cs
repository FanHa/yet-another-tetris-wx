using Model.Buffs;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Wind/Attack Boost")]
    public sealed class AttackBoostSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private BuffProjectile buffProjectilePrefab;
        [Header("Buff")]
        [SerializeField] private AttackBoostBuffDefinition attackBoostBuffDefinition;
        [Header("Cost")]
        [SerializeField] private float requiredEnergy = 55f;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;
        public AttackBoostBuffDefinition AttackBoostBuffDefinition => attackBoostBuffDefinition;
        public float RequiredEnergy => requiredEnergy;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(buffProjectilePrefab, nameof(buffProjectilePrefab));
            ValidateRequiredReference(attackBoostBuffDefinition, nameof(attackBoostBuffDefinition));
            ValidateFinitePositive(requiredEnergy, nameof(requiredEnergy));
        }
#endif
    }
}