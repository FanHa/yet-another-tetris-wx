using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Tetri
    {
        public event Action OnDataChanged;
        public enum TetriType
        {
            Character, // 角色类型
            Normal     // 普通类型
        }

        [SerializeField] private TetriType tetriType;
        public TetriType Type => tetriType;
        [SerializeField] private Serializable2DArray<Cell> shape;
        private Cell mainCell;

        public Serializable2DArray<Cell> Shape => shape;
        public Cell MainCell => mainCell;

        // TODO
        public int UpgradedTimes = 0;
        public const int MaxUpgradedTimes = 1;
        public const int MaxCharacterUpgradedTimes = 3;

        public Tetri(TetriType type)
        {

            this.tetriType = type;
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
                var previousCell = shape[row, column];
                shape[row, column] = cell;

                if (ReferenceEquals(previousCell, mainCell))
                {
                    mainCell = cell;
                }
            }
            else
            {
                Debug.LogWarning("Invalid row or column index.");
            }
        }

        public void SetMainCell(Cell cell)
        {
            if (cell == null || cell is Empty)
            {
                mainCell = null;
                return;
            }

            mainCell = cell;
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
            if (mainCell != null && !(mainCell is Empty))
            {
                return mainCell;
            }

            foreach (var pos in GetOccupiedPositions())
            {
                var cell = shape[pos.x, pos.y];
                if (cell is not Empty)
                {
                    return cell;
                }
            }

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

        public List<Cell> GetAllCells()
        {
            var cells = new List<Cell>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    var cell = shape[i, j];
                    if (cell is not Empty)
                    {
                        cells.Add(cell);
                    }
                }
            }
            return cells;
        }

        public Dictionary<AffinityType, int> GetAffinityCounts()
        {
            var result = new Dictionary<AffinityType, int>();
            foreach (var cell in GetAllCells())
            {
                var affinity = cell.Affinity;
                if (affinity == AffinityType.None)
                    continue;
                if (!result.ContainsKey(affinity))
                    result[affinity] = 0;
                result[affinity]++;
            }
            return result;
        }


        public void Rotate()
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

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    shape[rows - 1 - j, i] = tempShape[i, j];
                }
            }
        }

        public void UpgradeCoreCell()
        {
            // todo 判断tetri是不是在自己维护的列表中
            var mainCell = GetMainCell();
            mainCell.Level += 1;
            UpgradedTimes += 1;
            OnDataChanged?.Invoke();
        }

    }
}