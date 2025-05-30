using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTableModel", menuName = "Tetris/OperationTableModel")]
    public class OperationTableModel : ScriptableObject
    {
        public int Width;
        public int Height;
        public event Action OnChanged;

        [SerializeField]
        private List<PlacedTetri> placedTetris = new List<PlacedTetri>();

        public IReadOnlyList<PlacedTetri> PlacedTetris => placedTetris;

        private Model.Tetri.Cell[,] occupiedCellGrid;
        public Model.Tetri.Cell[,] OccupiedCellGrid => occupiedCellGrid; // Public getter for the grid


        /// <summary>
        /// 尝试放置一个 Tetri 到指定位置
        /// </summary>
        /// <param name="tetri">要放置的 Tetri</param>
        /// <param name="position">左上角在表格上的格子坐标</param>
        /// <returns>是否放置成功</returns>
        public bool TryPlaceTetri(Model.Tetri.Tetri tetri, Vector2Int position)
        {
            if (tetri == null)
            {
                Debug.LogError("尝试放置一个空的 Tetri。");
                return false;
            }
            if (occupiedCellGrid == null)
            {
                Debug.LogError("CellGrid 未初始化。请检查 OperationTableModel 的 Width/Height 设置。");
                return false;
            }

            var cellsToOccupyGlobal = new List<Vector2Int>();
            List<Vector2Int> tetriRelativeOccupiedPositions = tetri.GetOccupiedPositions();
            foreach (var relativePos in tetriRelativeOccupiedPositions)
            {
                cellsToOccupyGlobal.Add(position + relativePos);
            }

            foreach (var globalCellPos in cellsToOccupyGlobal)
            {
                if (globalCellPos.x < 0 || globalCellPos.x >= occupiedCellGrid.GetLength(0) ||
                    globalCellPos.y < 0 || globalCellPos.y >= occupiedCellGrid.GetLength(1))
                {
                    Debug.Log($"超出边界: {globalCellPos}");
                    return false;
                }
                // 直接检查 cellGrid
                if (occupiedCellGrid[globalCellPos.x, globalCellPos.y] != null)
                {
                    Debug.Log($"已被占用: {globalCellPos}");
                    return false;
                }
            }

            var placedTetri = new Model.PlacedTetri(tetri, position);

            placedTetris.Add(placedTetri);
            foreach (var relativePos in tetriRelativeOccupiedPositions)
            {
                Vector2Int globalPos = position + relativePos;
                // 不再需要添加到 occupiedCoordinates

                Model.Tetri.Cell cellObject = tetri.Shape[relativePos.x, relativePos.y];
                if (cellObject != null)
                {
                    if (globalPos.x >= 0 && globalPos.x < occupiedCellGrid.GetLength(0) &&
                        globalPos.y >= 0 && globalPos.y < occupiedCellGrid.GetLength(1))
                    {
                        occupiedCellGrid[globalPos.x, globalPos.y] = cellObject;
                    }
                }
            }

            OnChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            placedTetris.Clear();

            if (Width > 0 && Height > 0)
            {
                occupiedCellGrid = new Model.Tetri.Cell[Width, Height]; // 重新创建空的 grid
            }
            else
            {
                Debug.LogError("OperationTableModel Width or Height is not set correctly for Clear. Defaulting to 10x10 for grid re-initialization.");
                occupiedCellGrid = new Model.Tetri.Cell[10, 10];
            }

            OnChanged?.Invoke();
        }

        public void RemoveTetri(Model.PlacedTetri placedTetriToRemove)
        {
            if (placedTetriToRemove != null && placedTetris.Contains(placedTetriToRemove))
            {
                if (occupiedCellGrid == null)
                {
                    Debug.LogError("CellGrid is null during RemoveTetri. This should not happen.");
                    return;
                }

                List<Vector2Int> tetriRelativeOccupiedPositions = placedTetriToRemove.Tetri.GetOccupiedPositions();
                foreach (var relativePos in tetriRelativeOccupiedPositions)
                {
                    Vector2Int globalPos = placedTetriToRemove.Position + relativePos;
                    // 不再需要从 occupiedCoordinates 移除

                    if (globalPos.x >= 0 && globalPos.x < occupiedCellGrid.GetLength(0) &&
                        globalPos.y >= 0 && globalPos.y < occupiedCellGrid.GetLength(1))
                    {
                        occupiedCellGrid[globalPos.x, globalPos.y] = null; // 从 grid 中清除
                    }
                }

                placedTetris.Remove(placedTetriToRemove);
                OnChanged?.Invoke();
            }
            else
            {
                Debug.LogWarning("尝试移除一个不存在的 Tetri 或者传入的 PlacedTetri 为 null。");
            }
        }

        internal List<List<Cell>> GetCharacterCellGroups()
        {
            var result = new List<List<Cell>>();
            if (occupiedCellGrid == null) return result;

            int width = occupiedCellGrid.GetLength(0);
            int height = occupiedCellGrid.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = occupiedCellGrid[x, y];
                    if (cell == null) continue;

                    // 判断是否为 CharacterCell
                    if (cell is Model.Tetri.Character character)
                    {
                        var group = new List<Cell>();
                        var offsets = character.GetInfluenceOffsets();

                        foreach (var offset in offsets)
                        {
                            int nx = x + offset.x;
                            int ny = y + offset.y;
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                var neighbor = occupiedCellGrid[nx, ny];
                                if (neighbor != null && !group.Contains(neighbor))
                                {
                                    group.Add(neighbor);
                                }
                            }
                        }

                        result.Add(group);
                    }
                }
            }

            return result;
        }
    }
}