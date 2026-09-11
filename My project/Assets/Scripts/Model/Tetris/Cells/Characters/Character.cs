using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Character : Cell
    {
        private CharacterDefinition definitionData;

        public CharacterDefinition DefinitionData => definitionData;

        public virtual string CharacterKey => definitionData.Id;

        public override string Name()
        {
            return string.IsNullOrWhiteSpace(definitionData.DisplayName) ? base.Name() : definitionData.DisplayName;
        }

        [SerializeField] private string characterName;
        public string CharacterName => EnsureCharacterName();

        private string GenerateUniqueName()
        {
            return $"{Name()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        private string EnsureCharacterName()
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                characterName = GenerateUniqueName();
            }

            return characterName;
        }

        public override void Initialize(CellDefinition cellDefinition)
        {
            base.Initialize(cellDefinition);
            definitionData = cellDefinition as CharacterDefinition ?? throw new ArgumentException($"Expected CharacterDefinition for {GetType().Name}.", nameof(cellDefinition));
        }

        public override void Apply(Unit unit)
        {
            unit.Attributes = new Units.Attributes(
                moveSpeedBase: definitionData.MoveSpeedBase,
                attackPowerBase: definitionData.AttackPowerBase,
                maxHealthBase: definitionData.MaxHealthBase,
                attacksPerTenSecondsBase: definitionData.AttacksPerTenSecondsBase,
                energyPerSecondBase: definitionData.EnergyPerSecondBase,
                attackRange: definitionData.AttackRangeBase
            );

            unit.Attributes.MoveSpeed.AddPercentageModifier(this, definitionData.MoveSpeedPercentModifier);
            unit.Attributes.AttackPower.AddPercentageModifier(this, definitionData.AttackPowerPercentModifier);
            unit.Attributes.MaxHealth.AddPercentageModifier(this, definitionData.MaxHealthPercentModifier);
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, definitionData.AttacksPerTenSecondsPercentModifier);
            unit.Attributes.EnergyPerSecond.AddPercentageModifier(this, definitionData.EnergyPerSecondPercentModifier);
            unit.Attributes.AttackRange.AddPercentageModifier(this, definitionData.AttackRangePercentModifier);

            unit.name = EnsureCharacterName();
        }

        public override string Description()
        {
            return $"攻击力: {definitionData.AttackPowerBase}, 生命值: {definitionData.MaxHealthBase}, 攻击频率: {definitionData.AttacksPerTenSecondsBase}, 移动速度: {definitionData.MoveSpeedBase}";
        }

        public List<Vector2Int> GetInfluenceOffsets()
        {
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
                for (int dx = -1; dx <= 2; dx++)
                {
                    for (int dy = -1; dy <= 2; dy++)
                    {
                        var pos = new Vector2Int(dx, dy);
                        if (selfOffsets.Contains(pos)) continue;
                        if (dx == -1 || dx == 2 || dy == -1 || dy == 2)
                            offsets.Add(pos);
                    }
                }
            }

            if (Level >= 2)
            {
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
