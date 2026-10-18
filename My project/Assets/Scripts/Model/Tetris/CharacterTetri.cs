using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public sealed class CharacterTetri : Tetri
    {
        private static readonly ReadOnlyCollection<Vector2Int> occupiedPositions = Array.AsReadOnly(new[]
        {
            new Vector2Int(1, 1), new Vector2Int(1, 2),
            new Vector2Int(2, 1), new Vector2Int(2, 2)
        });

        public override TetriType Type => TetriType.Character;
        public Character Character { get; }
        public Vector2Int InfluenceAnchor => occupiedPositions[0];

        public CharacterTetri(Character character, int upgradedTimes = 0)
            : base(upgradedTimes)
        {
            Character = character;
        }

        public override IReadOnlyCollection<Vector2Int> GetOccupiedTetriGridPositions()
        {
            return occupiedPositions;
        }

        public override void Rotate()
        {
        }

        public void ValidateConfiguration()
        {
            if (Character == null)
            {
                throw new InvalidOperationException("A character Tetri must contain a character.");
            }

            if (UpgradedTimes < 0 || UpgradedTimes > MaxUpgradedTimes)
            {
                throw new InvalidOperationException("Character Tetri upgrade count is outside the allowed range.");
            }

            Character.ValidateConfiguration();
        }

        protected override void UpgradeContent()
        {
            Character.Level += 1;
        }
    }
}