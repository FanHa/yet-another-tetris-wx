using Model.Buffs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Power Curse/Power Curse")]
    public class PowerCurseSkillConfig : SkillConfig<PowerCurseLevelConfig>
    {
        [Header("Resources")]
        [SerializeField] private Units.Projectiles.PowerCurseZone zonePrefab;
        [SerializeField] private PowerCurseBuffDefinition powerCurseBuffDefinition;

        public Units.Projectiles.PowerCurseZone ZonePrefab => zonePrefab;
        public PowerCurseBuffDefinition PowerCurseBuffDefinition => powerCurseBuffDefinition;

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
        public float RadiusPerResonantCell = 0.3f;
        public float BaseZoneLifetime = 15f;
        public float ZoneLifetimePerResonantCell = 0.5f;

    }
}