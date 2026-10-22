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
        [SerializeField] private LifeShieldBuffDefinition lifeShieldBuffDefinition;

        [Header("护盾")]
        [SerializeField] private float baseShieldMultiplierPercent = 100f;
        [SerializeField] private float shieldMultiplierPercentPerResonantCell;
        [SerializeField] private float buffDuration;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;
        public LifeShieldBuffDefinition LifeShieldBuffDefinition => lifeShieldBuffDefinition;
        public float BaseShieldMultiplierPercent => baseShieldMultiplierPercent;
        public float ShieldMultiplierPercentPerResonantCell => shieldMultiplierPercentPerResonantCell;
        public float BuffDuration => buffDuration;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(buffProjectilePrefab, nameof(buffProjectilePrefab));
            ValidateRequiredReference(lifeShieldBuffDefinition, nameof(lifeShieldBuffDefinition));
            ValidateFiniteNonNegative(baseShieldMultiplierPercent, nameof(baseShieldMultiplierPercent));
            ValidateFiniteNonNegative(shieldMultiplierPercentPerResonantCell, nameof(shieldMultiplierPercentPerResonantCell));
            if (float.IsNaN(buffDuration) || float.IsInfinity(buffDuration) || (buffDuration != -1f && buffDuration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(buffDuration)} must be -1 (permanent) or finite and greater than zero.", this);
            }

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                ValidateFiniteNonNegative(levelConfig.LifeCostPercent, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.LifeCostPercent)}");
            }
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

    }
}
