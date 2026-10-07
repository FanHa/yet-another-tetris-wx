using UnityEngine;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Electric/Thunder Strike")]
    public class ThunderStrikeSkillConfig : SkillConfig<ThunderStrikeLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private ThunderStrikeProjectile projectilePrefab;
        [SerializeField] private BuffDefinition thunderStrikeBuffDefinition;

        public ThunderStrikeProjectile ProjectilePrefab => projectilePrefab;
        public BuffDefinition ThunderStrikeBuffDefinition => thunderStrikeBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(thunderStrikeBuffDefinition, nameof(thunderStrikeBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class ThunderStrikeLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("伤害")]
        [Tooltip("基础技能伤害")]
        public float BaseDamage = 80f;

        [Tooltip("每个电系格子增加的伤害")]
        public float DamagePerElectricCell = 18f;

        [Header("眩晕")]
        [Tooltip("基础眩晕持续时间")]
        public float BaseStunDuration = 0.7f;

        [Tooltip("每个电系格子增加的眩晕时长")]
        public float StunDurationPerElectricCell = 0.08f;

        [Tooltip("眩晕持续时间上限")]
        public float MaxStunDuration = 1.3f;

        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 90f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
