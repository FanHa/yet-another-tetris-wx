using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Electric/Chain Lightning")]
    public class ChainLightningSkillConfig : SkillConfig<ChainLightningLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private ChainLightning projectilePrefab;

        public ChainLightning ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                ValidateFiniteNonNegative(levelConfig.BaseDamage, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.BaseDamage)}");
                ValidateFiniteNonNegative(levelConfig.DamagePerResonantCell, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.DamagePerResonantCell)}");

                if (levelConfig.DamageIncreasePercentage < 0f)
                {
                    Debug.LogError(
                        $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.DamageIncreasePercentage)} must not be negative.",
                        this
                    );
                }

                ValidateFiniteNonNegative(levelConfig.BaseEnergyDisruptionAmount, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.BaseEnergyDisruptionAmount)}");
                ValidateFiniteNonNegative(levelConfig.EnergyDisruptionAmountPerResonantCell, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.EnergyDisruptionAmountPerResonantCell)}");
            }
        }
    #endif
    }

        [System.Serializable]
        public class ChainLightningLevelConfig : SkillLevelConfig, IRequiresEnergy
        {
            [Header("消耗")]
            public float RequiredEnergy = 90f;
            float IRequiresEnergy.RequiredEnergy => RequiredEnergy;

            [Header("弹射")]
            public float BaseDamage = 10f;
            [Tooltip("每个匹配共鸣 Cell 增加的首跳伤害")]
            public float DamagePerResonantCell = 4f;
            [Tooltip("每次弹射相对上一跳伤害的递增百分比")]
            public float DamageIncreasePercentage = 20f;
            public int MaxBounces = 5;
            public float Range = 5f;

            [Header("短路")]
            [Tooltip("每次命中削减的基础能量")]
            public float BaseEnergyDisruptionAmount = 5f;
            [Tooltip("每个匹配共鸣 Cell 增加的削能量")]
            public float EnergyDisruptionAmountPerResonantCell = 3f;
        }
}