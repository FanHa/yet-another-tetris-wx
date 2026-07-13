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