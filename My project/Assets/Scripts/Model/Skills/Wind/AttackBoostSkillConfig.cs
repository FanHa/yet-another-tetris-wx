using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Attack Boost")]
    public class AttackBoostSkillConfig : SkillConfig<AttackBoostLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private BuffProjectile buffProjectilePrefab;
        [SerializeField] private AttackBoostBuffDefinition attackBoostBuffDefinition;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;
        public AttackBoostBuffDefinition AttackBoostBuffDefinition => attackBoostBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(buffProjectilePrefab, nameof(buffProjectilePrefab));
            ValidateRequiredReference(attackBoostBuffDefinition, nameof(attackBoostBuffDefinition));
        }
    #endif
    }
    [System.Serializable]
    public class AttackBoostLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("通用")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}