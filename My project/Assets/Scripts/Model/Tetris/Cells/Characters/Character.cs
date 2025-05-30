using System;
using System.Collections.Generic;
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

        // todo 提供修改level 的方法
        private int level = 1;
        public int Level => level;


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

        public List<Vector2Int> GetInfluenceOffsets()
        {
            List<Vector2Int> offsets = new List<Vector2Int>{Vector2Int.zero};
            if (level == 1)
            {
                offsets.AddRange(new[] {
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1),
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0)
                });
            }
            else if (level == 2)
            {
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (!offsets.Contains(new Vector2Int(dx, dy)))
                            offsets.Add(new Vector2Int(dx, dy));
            }
            else if (level >= 3)
            {
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (!offsets.Contains(new Vector2Int(dx, dy)))
                            offsets.Add(new Vector2Int(dx, dy));
                offsets.AddRange(new[] {
                    new Vector2Int(0, 2),
                    new Vector2Int(0, -2),
                    new Vector2Int(2, 0),
                    new Vector2Int(-2, 0)
                });
            }
            return offsets;
        }

    }
}