using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/CharacterBaseStatConfig")]
    public class CharacterBaseStatConfig : SkillConfig
    {
        [Header("基础属性")]
        public float MoveSpeedBase = 2f;
        public float AttackPowerBase = 10f;
        public float MaxHealthBase = 100f;
        public float AttacksPerTenSecondsBase = 2.5f;

        [Header("通用战斗参数")]
        public float EnergyPerSecondBase = 5f;
        public float AttackRangeBase = 0.2f;
    }
}