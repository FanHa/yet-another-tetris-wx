using Model.Buffs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Energy Curse/Energy Curse")]
    public class EnergyCurseSkillConfig : SkillConfig<EnergyCurseLevelConfig>
    {
        [Header("Resources")]
        [SerializeField] private Units.Projectiles.EnergyCurseZone zonePrefab;
        [SerializeField] private EnergyCurseBuffDefinition energyCurseBuffDefinition;

        public Units.Projectiles.EnergyCurseZone ZonePrefab => zonePrefab;
        public EnergyCurseBuffDefinition EnergyCurseBuffDefinition => energyCurseBuffDefinition;

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
                ValidateFiniteNonNegative(levelConfig.RadiusPerResonantCell, LevelField(index, nameof(levelConfig.RadiusPerResonantCell)));
                ValidateFinitePositive(levelConfig.BaseZoneLifetime, LevelField(index, nameof(levelConfig.BaseZoneLifetime)));
                ValidateFiniteNonNegative(levelConfig.ZoneLifetimePerResonantCell, LevelField(index, nameof(levelConfig.ZoneLifetimePerResonantCell)));
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
        public float RadiusPerResonantCell = 0.3f;
        public float BaseZoneLifetime = 15f;
        public float ZoneLifetimePerResonantCell = 0.5f;
    }
}
