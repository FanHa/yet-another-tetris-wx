using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Units.Skills;
using Unity.VisualScripting;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Cell
    {
        public int Level { get; private set; } = 1;
        protected SkillConfigGroup skillConfigGroup;

        public void SetLevel(int level)
        {
            Level = Mathf.Max(1, level);
        }
        public virtual void SetLevelConfig(SkillConfigGroup skillConfigGroup)
        {
            this.skillConfigGroup = skillConfigGroup;
        }   


        public abstract string Description();
        public abstract string Name();

        public virtual void Apply(Units.Unit unit) { }
        public virtual void PostApply(Units.Unit unit, IReadOnlyList<Cell> allCells) { }

        public virtual Cell Clone()
        {
            // 如有特殊字段，子类重写 Clone()
            return (Cell)this.MemberwiseClone();
        }
        public AffinityType Affinity { get; set; } = AffinityType.None;

        // 通用的配置获取方法
        // protected TConfig GetConfigByLevel<TGroup, TConfig>(int level)
        //     where TGroup : SkillConfigGroup
        //     where TConfig : SkillConfig
        // {
        //     var levelConfigs = skillConfig.LevelConfigs.Cast<TConfig>().ToList();
        //     int idx = Mathf.Clamp(level - 1, 0, levelConfigs.Count - 1);
        //     return levelConfigs[idx];
        // }


    }
}