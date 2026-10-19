using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Electric/Electric Field")]
    public sealed class ElectricFieldSkillConfig : SkillConfig<ElectricFieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField] private ElectricFieldZone projectilePrefab;
        [SerializeField] private ThunderStrikeProjectile hitProjectilePrefab;

        public ElectricFieldZone ProjectilePrefab => projectilePrefab;
        public ThunderStrikeProjectile HitProjectilePrefab => hitProjectilePrefab;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(hitProjectilePrefab, nameof(hitProjectilePrefab));

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                string levelPrefix = $"{nameof(LevelConfigs)}[{index}]";
                ValidateFinitePositive(levelConfig.BaseRadius, $"{levelPrefix}.{nameof(levelConfig.BaseRadius)}");
                ValidateFiniteNonNegative(levelConfig.RadiusPerElectricCell, $"{levelPrefix}.{nameof(levelConfig.RadiusPerElectricCell)}");
                ValidateFinitePositive(levelConfig.BaseDuration, $"{levelPrefix}.{nameof(levelConfig.BaseDuration)}");
                ValidateFiniteNonNegative(levelConfig.DurationPerElectricCell, $"{levelPrefix}.{nameof(levelConfig.DurationPerElectricCell)}");
                ValidateFiniteNonNegative(levelConfig.BaseDamage, $"{levelPrefix}.{nameof(levelConfig.BaseDamage)}");
                ValidateFiniteNonNegative(levelConfig.DamagePerElectricCell, $"{levelPrefix}.{nameof(levelConfig.DamagePerElectricCell)}");
                ValidateFiniteNonNegative(levelConfig.BaseEnergyDisruptionAmount, $"{levelPrefix}.{nameof(levelConfig.BaseEnergyDisruptionAmount)}");
                ValidateFiniteNonNegative(levelConfig.EnergyDisruptionAmountPerElectricCell, $"{levelPrefix}.{nameof(levelConfig.EnergyDisruptionAmountPerElectricCell)}");
            }
        }
#endif
    }

    [System.Serializable]
    public sealed class ElectricFieldLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("区域")]
        public float BaseRadius = 2f;
        public float RadiusPerElectricCell = 0.1f;
        public float BaseDuration = 3f;
        public float DurationPerElectricCell = 0.2f;

        [Header("电击")]
        public float BaseDamage = 4f;
        public float DamagePerElectricCell = 4f;
        public float BaseEnergyDisruptionAmount = 5f;
        public float EnergyDisruptionAmountPerElectricCell = 5f;

        [Header("消耗")]
        public float RequiredEnergy = 90f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}