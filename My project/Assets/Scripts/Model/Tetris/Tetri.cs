using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Tetri
    {
        public event Action OnDataChanged;
        public enum TetriType
        {
            Character, // 角色类型
            CellBased  // 由 Cell 构成的 Tetri
        }

        public const int TetriGridSize = 4;
        public const int MaxUpgradedTimes = 1;

        public abstract TetriType Type { get; }
        public int UpgradedTimes { get; private set; }

        protected Tetri(int upgradedTimes = 0)
        {
            UpgradedTimes = upgradedTimes;
        }

        public abstract IReadOnlyCollection<Vector2Int> GetOccupiedTetriGridPositions();

        public abstract void Rotate();

        protected static Vector2Int RotateGridPosition(Vector2Int gridPosition)
        {
            return new Vector2Int(TetriGridSize - 1 - gridPosition.y, gridPosition.x);
        }

        public bool CanBeUpgraded()
        {
            return UpgradedTimes < MaxUpgradedTimes;
        }

        public void Upgrade()
        {
            if (!CanBeUpgraded())
            {
                return;
            }

            UpgradeContent();
            UpgradedTimes += 1;
            OnDataChanged?.Invoke();
        }

        protected abstract void UpgradeContent();
    }
}