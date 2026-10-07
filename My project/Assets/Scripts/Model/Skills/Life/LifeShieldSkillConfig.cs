using UnityEngine;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Shield")]
    public class LifeShieldSkillConfig : SkillConfig<LifeShieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private BuffProjectile buffProjectilePrefab;
        [SerializeField] private BuffDefinition lifeShieldBuffDefinition;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;
        public BuffDefinition LifeShieldBuffDefinition => lifeShieldBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(buffProjectilePrefab, nameof(buffProjectilePrefab));
            ValidateRequiredReference(lifeShieldBuffDefinition, nameof(lifeShieldBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class LifeShieldLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("通用")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;

        [Header("属性")]
        public float LifeCostPercent;
        public float AbsorbPercent;
        public float BuffDuration;

    }
}
