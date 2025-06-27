using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FrostZoneConfigGroup")]
    public class FrostZoneConfigGroup : SkillConfigGroup
    {
        public List<FrostZoneConfig> LevelConfigs;
    }

    [System.Serializable]
    public class FrostZoneConfig : SkillConfig
    {
        [Header("基础属性")]
        [Tooltip("基础半径")]
        public float BaseRadius = 1.5f;
        [Tooltip("基础持续时间")]
        public float BaseDuration = 5f;
        [Tooltip("基础伤害")]
        public float BaseDamage = 8f;
        [Tooltip("基础冰冻时长")]
        public float BaseChilledDuration = 2f;
        [Tooltip("基础移动减速百分比")]
        public int BaseChilledMoveSlowPercent = 10;

        [Header("每个冰块加成")]
        [Tooltip("每个冰块增加的半径")]
        public float RadiusPerIceCell = 0.2f;
        [Tooltip("每个冰块增加的持续时间")]
        public float DurationPerIceCell = 1f;
        [Tooltip("每个冰块增加的伤害")]
        public float DamagePerIceCell = 3f;
        [Tooltip("每个冰块增加的冰冻时长")]
        public float ChilledDurationPerIceCell = 0.5f;

        [Header("减速效果")]
        [Tooltip("每个冰块增加的移动减速百分比")]
        public int ChilledMoveSlowPercentPerIceCell = 3;
        [Tooltip("基础攻击减速百分比")]
        public int BaseChilledAtkSlowPercent = 10;
        [Tooltip("每个冰块增加的攻击减速百分比")]
        public int ChilledAtkSlowPercentPerIceCell = 3;
        [Tooltip("基础能量回复减速百分比")]
        public int BaseChilledEnergySlowPercent = 15;
        [Tooltip("每个冰块增加的能量回复减速百分比")]
        public int ChilledEnergySlowPercentPerIceCell = 4;

        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 120f;
    }
}