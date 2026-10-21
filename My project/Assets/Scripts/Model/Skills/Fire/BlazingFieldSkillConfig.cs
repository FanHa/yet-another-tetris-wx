using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Blazing Field")]
    public class BlazingFieldSkillConfig : SkillConfig<BlazingFieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private BlazingField projectilePrefab;

        public BlazingField ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class BlazingFieldLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("基础属性")]
        public float BaseRadius = 1.5f;
        public float BaseDuration = 5f;

        [Header("每个匹配共鸣 Cell 加成")]
        public float RadiusPerResonantCell = 0.2f;
        public float DurationPerResonantCell = 1f;

        [Header("消耗")]
        public float RequiredEnergy = 120f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
