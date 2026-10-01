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
#endif
    }

    public abstract class SkillConfig<TLevelConfig> : SkillConfig where TLevelConfig : SkillLevelConfig
    {
        [Header("等级配置")]
        public List<TLevelConfig> LevelConfigs = new();

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
}