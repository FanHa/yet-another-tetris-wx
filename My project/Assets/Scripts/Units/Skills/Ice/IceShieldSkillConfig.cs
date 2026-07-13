using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/IceShieldSkillConfig")]
    public class IceShieldSkillConfig : SkillConfig<IceShieldLevelConfig>
    {
    }

    [System.Serializable]
    public class IceShieldLevelConfig : SkillLevelConfig
    {
        [Header("减速效果")]
        public int BaseMoveSlowPercent = 10;
        public int MoveSlowPercentPerIceCell = 5;
        public int BaseAtkSlowPercent = 10;
        public int AtkSlowPercentPerIceCell = 5;
        public int BaseActionSlowPercent = 10;
        public int ActionSlowPercentPerIceCell = 5;
        public int BaseEnergySlowPercent = 15;
        public int EnergySlowPercentPerIceCell = 5;

        [Header("护盾与冰冻")]
        public float BuffDuration = -1f;
        public float BaseChilledDuration = 5f;
        public float ChilledDurationAdditionPerIceCell = 0.5f;
    }
}