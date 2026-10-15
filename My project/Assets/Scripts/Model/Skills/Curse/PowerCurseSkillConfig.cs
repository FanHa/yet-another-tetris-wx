using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Power Curse/Power Curse")]
    public class PowerCurseSkillConfig : SkillConfig<PowerCurseLevelConfig>
    {
        [Header("Resources")]
        [SerializeField] private Units.Projectiles.PowerCurseZone zonePrefab;
        [SerializeField] private BuffDefinition powerCurseBuffDefinition;

        public Units.Projectiles.PowerCurseZone ZonePrefab => zonePrefab;
        public BuffDefinition PowerCurseBuffDefinition => powerCurseBuffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(zonePrefab, nameof(zonePrefab));
            ValidateRequiredReference(powerCurseBuffDefinition, nameof(powerCurseBuffDefinition));
            if (powerCurseBuffDefinition != null && powerCurseBuffDefinition.Id != "buff.power_curse")
            {
                Debug.LogError($"{nameof(powerCurseBuffDefinition)} must reference the 'buff.power_curse' definition.", this);
            }

            if (LevelConfigs != null && LevelConfigs.Count == 0)
            {
                Debug.LogError($"{nameof(LevelConfigs)} must contain at least one level config.", this);
            }

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                ValidateFinitePositive(levelConfig.BaseRadius, LevelField(index, nameof(levelConfig.BaseRadius)));
                ValidateFinitePositive(levelConfig.BaseZoneLifetime, LevelField(index, nameof(levelConfig.BaseZoneLifetime)));
                ValidateFinitePositive(levelConfig.DebuffDuration, LevelField(index, nameof(levelConfig.DebuffDuration)));

                ValidatePercent(index, nameof(levelConfig.BaseAttackReductionPercent), levelConfig.BaseAttackReductionPercent);
                ValidateFiniteNonNegative(levelConfig.AttackReductionPerDeathCurseCell, LevelField(index, nameof(levelConfig.AttackReductionPerDeathCurseCell)));
                ValidatePercent(index, nameof(levelConfig.MaxAttackReductionPercent), levelConfig.MaxAttackReductionPercent);

                if (levelConfig.BaseAttackReductionPercent > levelConfig.MaxAttackReductionPercent)
                {
                    Debug.LogError($"{LevelField(index, nameof(levelConfig.BaseAttackReductionPercent))} must not exceed {nameof(levelConfig.MaxAttackReductionPercent)}.", this);
                }

            }
        }

        private void ValidatePercent(int index, string fieldName, float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 100f)
            {
                Debug.LogError($"{LevelField(index, fieldName)} must be finite and between 0 and 100.", this);
            }
        }

        private static string LevelField(int index, string fieldName) => $"LevelConfigs[{index}].{fieldName}";
#endif
    }

    [System.Serializable]
    public class PowerCurseLevelConfig : SkillLevelConfig
    {
        [Header("Area")]
        public float BaseRadius = 5f;
        public float RadiusPerDeathCurseCell = 0.3f;
        public float BaseZoneLifetime = 15f;
        public float ZoneLifetimePerDeathCurseCell = 0.5f;
        public float DebuffDuration = 1.5f;

        [Header("Attack Power Reduction (%)")]
        public float BaseAttackReductionPercent = 20f;
        public float AttackReductionPerDeathCurseCell = 2f;
        public float MaxAttackReductionPercent = 80f;

    }
}