using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : Cell
    {
        [SerializeField] private string characterName; // 永久角色名
        public string CharacterName => characterName; // 只读属性，获取角色名

        // 基础属性
        public float AttackPowerValue = 10f; // 默认攻击力
        public float MaxCoreValue = 100f;  // 默认最大生命值
        public float MoveSpeedValue = 1f;
        public float AttacksPerTenSeconds = 3f;
        public float RangeAttackDamagePercentage = 70f;
        public bool IsRanged = false;


        public Character()
        {
            // 生成唯一的角色名
            characterName = GenerateUniqueName();
        }
        private string GenerateUniqueName()
        {
            return $"{Name()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}"; // 生成基于角色类型和GUID的唯一名称
        }

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttackPower.SetBaseValue(AttackPowerValue);
            unit.Attributes.MaxHealth.SetBaseValue(MaxCoreValue);
            unit.Attributes.AttacksPerTenSeconds.SetBaseValue(AttacksPerTenSeconds);
            unit.Attributes.MoveSpeed.SetBaseValue(MoveSpeedValue);
            unit.Attributes.RangeAttackDamagePercentage = RangeAttackDamagePercentage;
            unit.Attributes.IsRanged = IsRanged;
            unit.name = characterName;
        }

        public override string Description()
        {
            return $"攻击力: {AttackPowerValue}, 生命值: {MaxCoreValue}, 攻击频率: {AttacksPerTenSeconds}, 移动速度: {MoveSpeedValue}";
        }

    }
}