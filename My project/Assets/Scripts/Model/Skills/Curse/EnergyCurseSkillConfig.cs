using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Energy Curse/Energy Curse")]
    public class EnergyCurseSkillConfig : SkillConfig<EnergyCurseLevelConfig>
    {
        [Header("Resources")]
        [SerializeField] private Units.Projectiles.EnergyCurseZone zonePrefab;
        [SerializeField] private BuffDefinition energyCurseBuffDefinition;

        public Units.Projectiles.EnergyCurseZone ZonePrefab => zonePrefab;
        public BuffDefinition EnergyCurseBuffDefinition => energyCurseBuffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(zonePrefab, nameof(zonePrefab));
            ValidateRequiredReference(energyCurseBuffDefinition, nameof(energyCurseBuffDefinition));
            if (energyCurseBuffDefinition != null && energyCurseBuffDefinition.Id != "buff.energy_curse")
            {
                Debug.LogError($"{nameof(energyCurseBuffDefinition)} must reference the 'buff.energy_curse' definition.", this);
            }

            if (LevelConfigs != null && LevelConfigs.Count == 0)
            {
                Debug.LogError($"{nameof(LevelConfigs)} must contain at least one level config.", this);
            }

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                ValidateFinitePositive(levelConfig.BaseRadius, LevelField(index, nameof(levelConfig.BaseRadius)));
                ValidateFiniteNonNegative(levelConfig.RadiusPerDeathCurseCell, LevelField(index, nameof(levelConfig.RadiusPerDeathCurseCell)));
                ValidateFinitePositive(levelConfig.BaseZoneLifetime, LevelField(index, nameof(levelConfig.BaseZoneLifetime)));
                ValidateFiniteNonNegative(levelConfig.ZoneLifetimePerDeathCurseCell, LevelField(index, nameof(levelConfig.ZoneLifetimePerDeathCurseCell)));
                ValidateFinitePositive(levelConfig.ApplyInterval, LevelField(index, nameof(levelConfig.ApplyInterval)));
                ValidateFinitePositive(levelConfig.DebuffDuration, LevelField(index, nameof(levelConfig.DebuffDuration)));

                ValidatePercent(index, nameof(levelConfig.BaseEnergyRegenReductionPercent), levelConfig.BaseEnergyRegenReductionPercent);
                ValidateFiniteNonNegative(levelConfig.EnergyRegenReductionPerDeathCurseCell, LevelField(index, nameof(levelConfig.EnergyRegenReductionPerDeathCurseCell)));
                ValidatePercent(index, nameof(levelConfig.MaxEnergyRegenReductionPercent), levelConfig.MaxEnergyRegenReductionPercent);

                if (levelConfig.BaseEnergyRegenReductionPercent > levelConfig.MaxEnergyRegenReductionPercent)
                {
                    Debug.LogError($"{LevelField(index, nameof(levelConfig.BaseEnergyRegenReductionPercent))} must not exceed {nameof(levelConfig.MaxEnergyRegenReductionPercent)}.", this);
                }

                if (levelConfig.ApplyInterval > levelConfig.BaseZoneLifetime)
                {
                    Debug.LogError($"{LevelField(index, nameof(levelConfig.ApplyInterval))} must not exceed {nameof(levelConfig.BaseZoneLifetime)}.", this);
                }

                if (levelConfig.DebuffDuration < levelConfig.ApplyInterval)
                {
                    Debug.LogError($"{LevelField(index, nameof(levelConfig.DebuffDuration))} must be at least {nameof(levelConfig.ApplyInterval)}.", this);
                }
            }
        }

        private void ValidatePercent(int index, string fieldName, float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 80f)
            {
                Debug.LogError($"{LevelField(index, fieldName)} must be finite and between 0 and 80.", this);
            }
        }

        private static string LevelField(int index, string fieldName) => $"LevelConfigs[{index}].{fieldName}";
#endif
    }

    [System.Serializable]
    public class EnergyCurseLevelConfig : SkillLevelConfig
    {
        [Header("Area")]
        public float BaseRadius = 5f;
        public float RadiusPerDeathCurseCell = 0.3f;
        public float BaseZoneLifetime = 15f;
        public float ZoneLifetimePerDeathCurseCell = 0.5f;
        public float ApplyInterval = 0.5f;
        public float DebuffDuration = 1.5f;

        [Header("Energy Regeneration Reduction (%)")]
        public float BaseEnergyRegenReductionPercent = 20f;
        public float EnergyRegenReductionPerDeathCurseCell = 2f;
        public float MaxEnergyRegenReductionPercent = 80f;
    }
}
