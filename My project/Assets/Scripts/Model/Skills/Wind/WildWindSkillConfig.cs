using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Wild Wind")]
    public class WildWindSkillConfig : SkillConfig<WildWindLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private WildWind projectilePrefab;
        [SerializeField] private BuffDefinition wildWindDebuffDefinition;

        public WildWind ProjectilePrefab => projectilePrefab;
        public BuffDefinition WildWindDebuffDefinition => wildWindDebuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(wildWindDebuffDefinition, nameof(wildWindDebuffDefinition));

                foreach (var (index, levelConfig) in GetValidLevelConfigs())
                {
                    string levelPrefix = $"{nameof(LevelConfigs)}[{index}]";
                    ValidateFiniteNonNegative(levelConfig.DamageAdditionPerResonantCell, $"{levelPrefix}.{nameof(levelConfig.DamageAdditionPerResonantCell)}");
                    ValidateFiniteNonNegative(levelConfig.RadiusPerResonantCell, $"{levelPrefix}.{nameof(levelConfig.RadiusPerResonantCell)}");
                    ValidateFiniteNonNegative(levelConfig.DurationPerResonantCell, $"{levelPrefix}.{nameof(levelConfig.DurationPerResonantCell)}");
                    ValidateFiniteNonNegative(levelConfig.DebuffDurationPerResonantCell, $"{levelPrefix}.{nameof(levelConfig.DebuffDurationPerResonantCell)}");
                    ValidateFiniteNonNegative(levelConfig.MoveSlowPercentPerResonantCell, $"{levelPrefix}.{nameof(levelConfig.MoveSlowPercentPerResonantCell)}");
                    ValidateFiniteNonNegative(levelConfig.AtkReducePercentPerResonantCell, $"{levelPrefix}.{nameof(levelConfig.AtkReducePercentPerResonantCell)}");
                }
        }
    #endif
    }

    [System.Serializable]
    public class WildWindLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("技能消耗")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;

        [Header("基础属性")]
        public float Damage;
        public float DebuffDuration;
        public int MoveSlowPercent;
        public int AtkReducePercent;
        public float Radius;
        public float Duration;

        [Header("加成")]
        public float DamageAdditionPerResonantCell = 3f; // 每个匹配共鸣 Cell 增加的伤害
        public float RadiusPerResonantCell = 0.1f;
        public float DurationPerResonantCell = 0.25f;
        public float DebuffDurationPerResonantCell = 0.2f;
        public int MoveSlowPercentPerResonantCell = 3;
        public int AtkReducePercentPerResonantCell = 3;
    }
}