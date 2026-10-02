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
        private void OnValidate()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

        [System.Serializable]
        public class ChainLightningLevelConfig : SkillLevelConfig
        {
            [Header("消耗")]
            public float RequiredEnergy = 90f;

            [Header("弹射")]
            public float BaseDamage = 20f;
            public float DamageIncreasePercentage = 20f;
            public int MaxBounces = 5;
            public float Range = 5f;
        }
}