using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/SnowballConfigGroup")]
    public class SnowballConfigGroup : SkillConfigGroup
    {
        public List<SnowballConfig> LevelConfigs;
    }

    [System.Serializable]
    public class SnowballConfig : SkillConfig
    {
        [Header("伤害")]
        public float BaseDamage = 10f;
        public float IceCellDamageBonus = 4f;

        [Header("Chilled效果")]
        public float BaseChilledDuration = 3f;
        public float ChilledDurationPerIceCell = 0.5f;
        public int BaseChilledMoveSlowPercent = 10;
        public int ChilledMoveSlowPercentPerIceCell = 3;
        public int BaseChilledAtkSlowPercent = 10;
        public int ChilledAtkSlowPercentPerIceCell = 3;
        public int BaseChilledEnergySlowPercent = 15;
        public int ChilledEnergySlowPercentPerIceCell = 4;

        [Header("消耗")]
        public float RequiredEnergy = 40f;
    }
}
