using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public abstract class SkillConfig : ScriptableObject
    {
    }

    public abstract class SkillConfig<TLevelConfig> : SkillConfig where TLevelConfig : SkillLevelConfig
    {
        [Header("等级配置")]
        public List<TLevelConfig> LevelConfigs = new();

        public bool TryGetLevelConfig(int level, out TLevelConfig config)
        {
            config = default;
            if (LevelConfigs == null || LevelConfigs.Count == 0)
            {
                return false;
            }

            int index = level - 1;
            if (index < 0 || index >= LevelConfigs.Count)
            {
                return false;
            }

            config = LevelConfigs[index];
            return config != null;
        }
    }

    public abstract class SkillLevelConfig
    {
    }
}