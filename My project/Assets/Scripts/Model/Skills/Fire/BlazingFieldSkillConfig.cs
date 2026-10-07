using UnityEngine;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Blazing Field")]
    public class BlazingFieldSkillConfig : SkillConfig<BlazingFieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private BlazingField projectilePrefab;
        [SerializeField] private BuffDefinition burnBuffDefinition;

        public BlazingField ProjectilePrefab => projectilePrefab;
        public BuffDefinition BurnBuffDefinition => burnBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(burnBuffDefinition, nameof(burnBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class BlazingFieldLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("基础属性")]
        public float BaseRadius = 1.5f;
        public float BaseDuration = 5f;
        public float BaseDotDps = 5f;
        public float BaseDotDuration = 3f;

        [Header("每个火块加成")]
        public float RadiusPerFireCell = 0.2f;
        public float DurationPerFireCell = 1f;
        public float DotDpsPerFireCell = 2f;
        public float DotDurationPerFireCell = 0.5f;

        [Header("消耗")]
        public float RequiredEnergy = 120f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
