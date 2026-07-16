using UnityEngine;
using Units.Skills;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "CharacterConfig/CharacterBaseStatConfig")]
    public class CharacterBaseStatConfig : SkillConfig
    {
        [Header("展示信息")]
        public string DisplayName;

        [TextArea]
        public string Description;

        [Header("基础属性")]
        public float MoveSpeedBase = 2f;
        public float AttackPowerBase = 10f;
        public float MaxHealthBase = 100f;
        public float AttacksPerTenSecondsBase = 2.5f;

        [Header("通用战斗参数")]
        public float EnergyPerSecondBase = 5f;
        public float AttackRangeBase = 0.2f;

        [Header("百分比修正")]
        public float MoveSpeedPercentModifier;
        public float AttackPowerPercentModifier;
        public float MaxHealthPercentModifier;
        public float AttacksPerTenSecondsPercentModifier;
        public float EnergyPerSecondPercentModifier;
        public float AttackRangePercentModifier;
    }
}