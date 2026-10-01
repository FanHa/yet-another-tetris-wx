using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Electric/Thunder Strike")]
    public class ThunderStrikeSkillConfig : SkillConfig<ThunderStrikeLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private ThunderStrikeProjectile projectilePrefab;

        public ThunderStrikeProjectile ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class ThunderStrikeLevelConfig : SkillLevelConfig
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
    }
}
