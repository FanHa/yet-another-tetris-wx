using UnityEngine;
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
                if (levelConfig.DamageIncreasePercentage < 0f)
                {
                    Debug.LogError(
                        $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.DamageIncreasePercentage)} must not be negative.",
                        this
                    );
                }
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
            public float BaseDamage = 20f;
            [Tooltip("每次弹射相对上一跳伤害的递增百分比")]
            public float DamageIncreasePercentage = 20f;
            public int MaxBounces = 5;
            public float Range = 5f;
        }
}