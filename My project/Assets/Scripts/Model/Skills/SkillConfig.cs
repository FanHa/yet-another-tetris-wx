using System.Collections.Generic;
using UnityEngine;

namespace Model.Skills
{
    public abstract class SkillConfig : ScriptableObject
    {
#if UNITY_EDITOR
        protected void ValidateRequiredReference(Object reference, string fieldName)
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            string configLabel = string.IsNullOrEmpty(assetPath)
                ? $"{GetType().Name} '{name}'"
                : $"{GetType().Name} '{name}' ({assetPath})";

            if (reference == null)
            {
                Debug.LogError($"{configLabel}.{fieldName} is not assigned.", this);
            }
        }

        protected void ValidateFiniteNonNegative(float value, string fieldName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and non-negative.", this);
            }
        }

        protected void ValidateFinitePositive(float value, string fieldName)
        {
            if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and greater than zero.", this);
            }
        }
#endif
    }

    public abstract class SkillConfig<TLevelConfig> : SkillConfig where TLevelConfig : SkillLevelConfig
    {
        [Header("等级配置")]
        public List<TLevelConfig> LevelConfigs = new();

#if UNITY_EDITOR
        protected void OnValidate()
        {
            ValidateLevelConfigs();
            ValidateSpecificConfig();
        }

        protected virtual void ValidateSpecificConfig()
        {
        }

        private void ValidateLevelConfigs()
        {
            if (LevelConfigs == null)
            {
                Debug.LogError($"{nameof(LevelConfigs)} is not assigned.", this);
                return;
            }

            for (int index = 0; index < LevelConfigs.Count; index++)
            {
                var levelConfig = LevelConfigs[index];
                if (levelConfig == null)
                {
                    Debug.LogError($"{nameof(LevelConfigs)}[{index}] is not assigned.", this);
                    continue;
                }

                if (levelConfig is IRequiresEnergy energyLevel)
                {
                    ValidateFinitePositive(energyLevel.RequiredEnergy, $"{nameof(LevelConfigs)}[{index}].{nameof(energyLevel.RequiredEnergy)}");
                }
            }
        }

        protected IEnumerable<(int Index, TLevelConfig Level)> GetValidLevelConfigs()
        {
            if (LevelConfigs == null)
            {
                yield break;
            }

            for (int index = 0; index < LevelConfigs.Count; index++)
            {
                if (LevelConfigs[index] != null)
                {
                    yield return (index, LevelConfigs[index]);
                }
            }
        }
#endif

        public TLevelConfig GetLevelConfig(int level)
        {
            if (LevelConfigs == null || LevelConfigs.Count == 0)
            {
                return default;
            }

            int index = level - 1;
            if (index < 0 || index >= LevelConfigs.Count)
            {
                return default;
            }

            return LevelConfigs[index];
        }
    }

    public abstract class SkillLevelConfig
    {
    }

    public interface IRequiresEnergy
    {
        float RequiredEnergy { get; }
    }
}