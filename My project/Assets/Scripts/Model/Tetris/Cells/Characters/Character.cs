using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.Padding;
        public abstract CharacterTypeId CharacterTypeId { get; }
        [SerializeField] private string characterName; // 永久角色名
        public string CharacterName => characterName; // 只读属性，获取角色名

        // 基础属性
        public float AttackPowerValue = 10f; // 默认攻击力
        public float MaxCoreValue = 100f;  // 默认最大生命值
        public float MoveSpeedValue = 1f;
        public float AttacksPerTenSeconds = 3f;

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
            unit.Attributes = new Units.Attributes(
                moveSpeedBase: 2f,
                attackPowerBase: 10f,
                maxHealthBase: 100f,
                attacksPerTenSecondsBase: 2.5f,
                energyPerSecondBase: 5f,
                attackRange: 1f
            );
            unit.name = characterName;
        }

        public override string Description()
        {
            return $"攻击力: {AttackPowerValue}, 生命值: {MaxCoreValue}, 攻击频率: {AttacksPerTenSeconds}, 移动速度: {MoveSpeedValue}";
        }

        public List<Vector2Int> GetInfluenceOffsets()
        {
            // 本体
            var selfOffsets = new HashSet<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1)
            };

            var offsets = new List<Vector2Int>(selfOffsets);

            if (Level >= 1)
            {
                // Level1: 4x4外围一圈
                for (int dx = -1; dx <= 2; dx++)
                {
                    for (int dy = -1; dy <= 2; dy++)
                    {
                        var pos = new Vector2Int(dx, dy);
                        // 跳过本体
                        if (selfOffsets.Contains(pos)) continue;
                        // 只加外围
                        if (dx == -1 || dx == 2 || dy == -1 || dy == 2)
                            offsets.Add(pos);
                    }
                }
            }

            if (Level >= 2)
            {
                // Level2: 上下左右各延申2格
                offsets.Add(new Vector2Int(-2, 0));
                offsets.Add(new Vector2Int(-2, 1));
                offsets.Add(new Vector2Int(2, 0));
                offsets.Add(new Vector2Int(2, 1));
                offsets.Add(new Vector2Int(0, -2));
                offsets.Add(new Vector2Int(1, -2));
                offsets.Add(new Vector2Int(0, 2));
                offsets.Add(new Vector2Int(1, 2));
            }

            return offsets;
        }

    }
}