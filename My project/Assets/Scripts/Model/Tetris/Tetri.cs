using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Tetri
    {
        public enum TetriType
        {
            Character, // 角色类型
            Normal     // 普通类型
        }

        // public CellGroupConfig.Group Group;
        [SerializeField] private TetriType tetriType;
        public TetriType Type => tetriType;
        [SerializeField] private Serializable2DArray<Cell> shape;

        public Serializable2DArray<Cell> Shape => shape;
        [SerializeField] private bool isDisposable;
        public bool IsDisposable => isDisposable;

        // TODO
        public int UpgradedTimes = 0;
        public const int MaxUpgradedTimes = 1;
        public const int MaxCharacterUpgradedTimes = 3;

        public Tetri(TetriType type, bool isDisposable = false)
        {

            this.tetriType = type;
            this.isDisposable = isDisposable;
            InitializeShape(4, 4); // 默认大小为 4x4
        }
        private void InitializeShape(int rows, int cols)
        {
            shape = new Serializable2DArray<Cell>(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    shape[i, j] = new Empty();
                }
            }
        }
        public void SetCell(int row, int column, Cell cell)
        {
            if (row >= 0 && row < shape.GetLength(0) && column >= 0 && column < shape.GetLength(1))
            {
                shape[row, column] = cell;
            }
            else
            {
                Debug.LogWarning("Invalid row or column index.");
            }
        }

        public void ReplaceRandomOccupiedCell(Model.Tetri.Cell cellInstance)
        {
            var occupiedPositions = this.GetOccupiedPositions(); 

            if (occupiedPositions.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, occupiedPositions.Count); 
                var randomCellPosition = occupiedPositions[randomIndex];
                this.SetCell(randomCellPosition.x, randomCellPosition.y, cellInstance);
            }
            else
            {
                Debug.LogWarning($"Tetri has no occupied cells to replace.");
            }
        }

        public List<Vector2Int> GetOccupiedPositions()
        {
            var occupiedPositions = new List<Vector2Int>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] is not Empty)
                    {
                        occupiedPositions.Add(new Vector2Int(i, j));
                    }
                }
            }
            return occupiedPositions;
        }

        public Cell GetMainCell()
        {
            var occupiedPositions = GetOccupiedPositions();
            foreach (var pos in occupiedPositions)
            {
                var cell = shape[pos.x, pos.y];
                if (cell is not Padding)
                {
                    return cell;
                }
            }
            // 没有主Cell，查找已有的Padding
            foreach (var pos in occupiedPositions)
            {
                var cell = shape[pos.x, pos.y];
                if (cell is Padding)
                {
                    return cell;
                }
            }
            // 没有主Cell也没有Padding
            return null;
        }

        public bool CanBeUpgraded()
        {
            if (tetriType == TetriType.Character && UpgradedTimes >= MaxCharacterUpgradedTimes)
            {
                return false;
            }
            return UpgradedTimes < MaxUpgradedTimes;
        }

        internal void Rotate()
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            var tempShape = new Serializable2DArray<Cell>(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tempShape[i, j] = shape[i, j];
                }
            }

            // 就地旋转
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    shape[j, rows - 1 - i] = tempShape[i, j];
                }
            }
        }

        internal Tetri Clone()
        {
            var clone = new Tetri(this.tetriType, this.isDisposable)
            {
                UpgradedTimes = this.UpgradedTimes
            };

            // 深拷贝 shape
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // 假设 Cell 有 Clone 方法，否则需要你实现
                    clone.shape[i, j] = shape[i, j]?.Clone() ?? null;
                }
            }

            return clone;
        }
    }
}