using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Characters/Character Definition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string id;
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

        private static readonly Regex IdPattern = new("^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = name;
            }

            ValidateConfiguration();
        }
#endif

        public bool ValidateConfiguration()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(id) ||
                !string.Equals(id, id.Trim(), StringComparison.Ordinal) || !IdPattern.IsMatch(id))
            {
                Debug.LogError($"CharacterDefinition ID '{id}' is invalid. Expected pattern: {IdPattern}.", this);
                isValid = false;
            }

            if (energyPerSecondBase < 0f || float.IsNaN(energyPerSecondBase) || float.IsInfinity(energyPerSecondBase))
            {
                Debug.LogError($"{nameof(energyPerSecondBase)} must be finite and non-negative.", this);
                isValid = false;
            }

            if (float.IsNaN(energyPerSecondPercentModifier) || float.IsInfinity(energyPerSecondPercentModifier))
            {
                Debug.LogError($"{nameof(energyPerSecondPercentModifier)} must be finite.", this);
                isValid = false;
            }

            return isValid;
        }

        public string Id => id;

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

        public Sprite Icon => icon;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? null : displayName;
        public string Description => string.IsNullOrWhiteSpace(description) ? null : description;
    }
}
