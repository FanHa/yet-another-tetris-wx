using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Characters/Character Definition")]
    public sealed class CharacterDefinition : CellDefinition
    {
        [SerializeField] private Sprite icon;

        [Header("展示信息")]
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;

        [Header("基础属性")]
        [SerializeField] private float moveSpeedBase = 2f;
        [SerializeField] private float attackPowerBase = 10f;
        [SerializeField] private float maxHealthBase = 100f;
        [SerializeField] private float attacksPerTenSecondsBase = 2.5f;

        [Header("通用战斗参数")]
        [SerializeField] private float energyPerSecondBase = 5f;
        [SerializeField] private float attackRangeBase = 0.2f;

        [Header("百分比修正")]
        [SerializeField] private float moveSpeedPercentModifier;
        [SerializeField] private float attackPowerPercentModifier;
        [SerializeField] private float maxHealthPercentModifier;
        [SerializeField] private float attacksPerTenSecondsPercentModifier;
        [SerializeField] private float energyPerSecondPercentModifier;
        [SerializeField] private float attackRangePercentModifier;

        public float MoveSpeedBase => moveSpeedBase;
        public float AttackPowerBase => attackPowerBase;
        public float MaxHealthBase => maxHealthBase;
        public float AttacksPerTenSecondsBase => attacksPerTenSecondsBase;
        public float EnergyPerSecondBase => energyPerSecondBase;
        public float AttackRangeBase => attackRangeBase;

        public float MoveSpeedPercentModifier => moveSpeedPercentModifier;
        public float AttackPowerPercentModifier => attackPowerPercentModifier;
        public float MaxHealthPercentModifier => maxHealthPercentModifier;
        public float AttacksPerTenSecondsPercentModifier => attacksPerTenSecondsPercentModifier;
        public float EnergyPerSecondPercentModifier => energyPerSecondPercentModifier;
        public float AttackRangePercentModifier => attackRangePercentModifier;

        public override Sprite Icon => icon;
        public override string DisplayName => string.IsNullOrWhiteSpace(displayName) ? base.DisplayName : displayName;
        public override string Description => string.IsNullOrWhiteSpace(description) ? base.Description : description;
    }
}
