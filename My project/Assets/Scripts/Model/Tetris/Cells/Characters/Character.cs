using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : Cell
    {
        private CharacterBaseStatConfig ResolveConfig()
        {
            return (CharacterBaseStatConfig)Config;
        }

        public override string Name()
        {
            return ResolveConfig().DisplayName;
        }

        public override CellTypeId CellTypeId => CellTypeId.Padding;
        public abstract CharacterTypeId CharacterTypeId { get; }
        [SerializeField] private string characterName; // 永久角色名
        public string CharacterName => EnsureCharacterName(); // 只读属性，获取角色名

        private string GenerateUniqueName()
        {
            return $"{Name()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}"; // 生成基于角色类型和GUID的唯一名称
        }

        private string EnsureCharacterName()
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                characterName = GenerateUniqueName();
            }

            return characterName;
        }

        public override void Apply(Unit unit)
        {
            var config = ResolveConfig();
            unit.Attributes = new Units.Attributes(
                moveSpeedBase: config.MoveSpeedBase,
                attackPowerBase: config.AttackPowerBase,
                maxHealthBase: config.MaxHealthBase,
                attacksPerTenSecondsBase: config.AttacksPerTenSecondsBase,
                energyPerSecondBase: config.EnergyPerSecondBase,
                attackRange: config.AttackRangeBase
            );

            unit.Attributes.MoveSpeed.AddPercentageModifier(this, config.MoveSpeedPercentModifier);
            unit.Attributes.AttackPower.AddPercentageModifier(this, config.AttackPowerPercentModifier);
            unit.Attributes.MaxHealth.AddPercentageModifier(this, config.MaxHealthPercentModifier);
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, config.AttacksPerTenSecondsPercentModifier);
            unit.Attributes.EnergyPerSecond.AddPercentageModifier(this, config.EnergyPerSecondPercentModifier);
            unit.Attributes.AttackRange.AddPercentageModifier(this, config.AttackRangePercentModifier);

            unit.name = EnsureCharacterName();
        }

        public override string Description()
        {
            var config = ResolveConfig();
            return $"攻击力: {config.AttackPowerBase}, 生命值: {config.MaxHealthBase}, 攻击频率: {config.AttacksPerTenSecondsBase}, 移动速度: {config.MoveSpeedBase}";
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
