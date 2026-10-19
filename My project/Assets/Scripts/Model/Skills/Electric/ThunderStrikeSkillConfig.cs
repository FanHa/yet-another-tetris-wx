using UnityEngine;
using UnityEngine.Serialization;
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
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                ValidateFiniteNonNegative(levelConfig.BaseDamage, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.BaseDamage)}");
                ValidateFiniteNonNegative(levelConfig.DamagePerResonantCell, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.DamagePerResonantCell)}");

                ValidateFiniteNonNegative(levelConfig.BaseEnergyDisruptionAmount, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.BaseEnergyDisruptionAmount)}");
                ValidateFiniteNonNegative(levelConfig.EnergyDisruptionAmountPerResonantCell, $"{nameof(LevelConfigs)}[{index}].{nameof(levelConfig.EnergyDisruptionAmountPerResonantCell)}");
            }
        }
    #endif
    }

    [System.Serializable]
    public class ThunderStrikeLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("伤害")]
        [Tooltip("基础技能伤害")]
        public float BaseDamage = 10f;

        [Tooltip("每个匹配共鸣 Cell 增加的伤害")]
        public float DamagePerResonantCell = 8f;

        [Header("短路")]
        [Tooltip("每次命中削减的基础能量")]
        public float BaseEnergyDisruptionAmount = 5f;
        [Tooltip("每个匹配共鸣 Cell 增加的削能量")]
        public float EnergyDisruptionAmountPerResonantCell = 3f;

        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 90f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
