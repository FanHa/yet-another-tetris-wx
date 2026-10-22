using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Frost Zone")]
    public class FrostZoneSkillConfig : SkillConfig<FrostZoneLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private FrostZone projectilePrefab;

        public FrostZone ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class FrostZoneLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("基础属性")]
        [Tooltip("基础半径")]
        public float BaseRadius = 1.5f;
        [Tooltip("基础持续时间")]
        public float BaseDuration = 5f;
        [Tooltip("基础伤害")]
        public float BaseDamage = 8f;
        [Header("匹配共鸣 Cell 加成")]
        [Tooltip("匹配共鸣 Cell 增加的半径")]
        public float RadiusPerResonantCell = 0.2f;
        [Tooltip("匹配共鸣 Cell 增加的持续时间")]
        public float DurationPerResonantCell = 1f;
        [Tooltip("匹配共鸣 Cell 增加的伤害")]
        public float DamagePerResonantCell = 3f;


        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 120f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}