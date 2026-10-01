using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Shield")]
    public class LifeShieldSkillConfig : SkillConfig<LifeShieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private BuffProjectile buffProjectilePrefab;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateRequiredReference(buffProjectilePrefab, nameof(buffProjectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class LifeShieldLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float LifeCostPercent;
        public float AbsorbPercent;
        public float BuffDuration;

    }
}
