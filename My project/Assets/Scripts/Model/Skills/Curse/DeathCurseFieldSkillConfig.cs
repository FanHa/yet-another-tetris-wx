using Model.Buffs;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Curse/Death Curse Field")]
    public class DeathCurseFieldSkillConfig : SkillConfig<DeathCurseFieldLevelConfig>
    {
        [Header("Resources")]
        [SerializeField] private Units.Projectiles.CurseFieldZone zonePrefab;
        [SerializeField] private BuffDefinition curseBuffDefinition;

        public Units.Projectiles.CurseFieldZone ZonePrefab => zonePrefab;
        public BuffDefinition CurseBuffDefinition => curseBuffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(zonePrefab, nameof(zonePrefab));
            ValidateRequiredReference(curseBuffDefinition, nameof(curseBuffDefinition));
            if (curseBuffDefinition != null && curseBuffDefinition.Id != "buff.curse")
            {
                Debug.LogError($"{nameof(curseBuffDefinition)} must reference the 'buff.curse' definition.", this);
            }

            if (LevelConfigs != null && LevelConfigs.Count == 0)
            {
                Debug.LogError($"{nameof(LevelConfigs)} must contain at least one level config.", this);
            }

            foreach (var (index, levelConfig) in GetValidLevelConfigs())
            {
                ValidateFinitePositive(levelConfig.BaseRadius, LevelField(index, nameof(levelConfig.BaseRadius)));
                ValidateFinitePositive(levelConfig.BaseZoneLifetime, LevelField(index, nameof(levelConfig.BaseZoneLifetime)));
                ValidateFinitePositive(levelConfig.ApplyInterval, LevelField(index, nameof(levelConfig.ApplyInterval)));
                ValidateFinitePositive(levelConfig.DebuffDuration, LevelField(index, nameof(levelConfig.DebuffDuration)));

                ValidatePercent(index, nameof(levelConfig.BaseAttackReductionPercent), levelConfig.BaseAttackReductionPercent);
                ValidateFiniteNonNegative(levelConfig.AttackReductionPerCurseCell, LevelField(index, nameof(levelConfig.AttackReductionPerCurseCell)));
                ValidatePercent(index, nameof(levelConfig.MaxAttackReductionPercent), levelConfig.MaxAttackReductionPercent);
                ValidatePercent(index, nameof(levelConfig.BaseEnergyRegenReductionPercent), levelConfig.BaseEnergyRegenReductionPercent);
                ValidateFiniteNonNegative(levelConfig.EnergyRegenReductionPerCurseCell, LevelField(index, nameof(levelConfig.EnergyRegenReductionPerCurseCell)));
                ValidatePercent(index, nameof(levelConfig.MaxEnergyRegenReductionPercent), levelConfig.MaxEnergyRegenReductionPercent);

                if (levelConfig.BaseAttackReductionPercent > levelConfig.MaxAttackReductionPercent)
                {
                    Debug.LogError($"{LevelField(index, nameof(levelConfig.BaseAttackReductionPercent))} must not exceed {nameof(levelConfig.MaxAttackReductionPercent)}.", this);
                }

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
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 100f)
            {
                Debug.LogError($"{LevelField(index, fieldName)} must be finite and between 0 and 100.", this);
            }
        }

        private static string LevelField(int index, string fieldName) => $"LevelConfigs[{index}].{fieldName}";
#endif
    }

    [System.Serializable]
    public class DeathCurseFieldLevelConfig : SkillLevelConfig
    {
        [Header("Area")]
        public float BaseRadius = 5f;
        public float RadiusPerCurseCell = 0.3f;
        public float BaseZoneLifetime = 15f;
        public float ZoneLifetimePerCurseCell = 0.5f;
        public float ApplyInterval = 0.5f;
        public float DebuffDuration = 1.5f;

        [Header("Attack Power Reduction (%)")]
        public float BaseAttackReductionPercent = 20f;
        public float AttackReductionPerCurseCell = 2f;
        public float MaxAttackReductionPercent = 80f;

        [Header("Energy Regeneration Reduction (%)")]
        public float BaseEnergyRegenReductionPercent = 30f;
        public float EnergyRegenReductionPerCurseCell = 3f;
        public float MaxEnergyRegenReductionPercent = 80f;
    }
}