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
        // private List<PlacedTetri> placedTetris = new List<PlacedTetri>();
        // public IReadOnlyList<PlacedTetri> PlacedTetris => placedTetris;

        private Dictionary<Model.Tetri.Tetri, Vector2Int> placedMap = new();
        public IReadOnlyDictionary<Model.Tetri.Tetri, Vector2Int> PlacedMap => placedMap;

        private Model.Tetri.Cell[,] occupiedCellGrid;

        /// <summary>
        /// 尝试放置一个 Tetri 到指定位置
        /// </summary>
        /// <param name="tetri">要放置的 Tetri</param>
        /// <param name="position">左上角在表格上的格子坐标</param>
        /// <returns>是否放置成功</returns>
        public bool TryPlaceTetri(Model.Tetri.Tetri tetri, Vector2Int position)
        {
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

            tetri.OnDataChanged += HandleTetriChanged;
            foreach (var relativePos in tetriRelativeOccupiedPositions)
            {
                Vector2Int globalPos = position + relativePos;
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
            placedMap[tetri] = position;

            OnChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            foreach (var kv in placedMap)
            {
                kv.Key.OnDataChanged -= HandleTetriChanged;
            }
            placedMap.Clear();

            occupiedCellGrid = new Model.Tetri.Cell[Width, Height]; // 重新创建空的 grid
            OnChanged?.Invoke();
        }

        public void RemoveTetri(Model.Tetri.Tetri placedTetriToRemove)
        {
            placedMap.TryGetValue(placedTetriToRemove, out var position);
            placedTetriToRemove.OnDataChanged -= HandleTetriChanged;
            List<Vector2Int> tetriRelativeOccupiedPositions = placedTetriToRemove.GetOccupiedPositions();
            foreach (var relativePos in tetriRelativeOccupiedPositions)
            {
                Vector2Int globalPos = position + relativePos;
                // 不再需要从 occupiedCoordinates 移除
                if (globalPos.x >= 0 && globalPos.x < occupiedCellGrid.GetLength(0) &&
                    globalPos.y >= 0 && globalPos.y < occupiedCellGrid.GetLength(1))
                {
                    occupiedCellGrid[globalPos.x, globalPos.y] = null; // 从 grid 中清除
                }
            }
            placedMap.Remove(placedTetriToRemove);
            OnChanged?.Invoke();

        }

        private void HandleTetriChanged()
        {
            // Tetri 变动时，通知外部刷新
            OnChanged?.Invoke();
        }

        public List<CharacterInfluenceGroup> GetCharacterInfluenceGroups()
        {
            var result = new List<CharacterInfluenceGroup>();
            int width = occupiedCellGrid.GetLength(0);
            int height = occupiedCellGrid.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = occupiedCellGrid[x, y];
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
                        var characterGroup = new CharacterInfluenceGroup(character, group);
                        result.Add(characterGroup);
                    }
                }
            }

            return result;
        }

        public CharacterInfluenceGroup GetCharacterInfluenceGroupOf(Model.Tetri.Tetri tetri)
        {
            placedMap.TryGetValue(tetri, out var position);
            var basePosition = position + Vector2Int.one; // 主格子在 tetri 内的位置
            int width = occupiedCellGrid.GetLength(0);
            int height = occupiedCellGrid.GetLength(1);
            var group = new List<Cell>();
            var character = tetri.GetMainCell() as Model.Tetri.Character;
            var offsets = character.GetInfluenceOffsets();
            foreach (var offset in offsets)
            {
                int nx = basePosition.x + offset.x;
                int ny = basePosition.y + offset.y;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    var neighbor = occupiedCellGrid[nx, ny];
                    if (neighbor != null && !group.Contains(neighbor))
                    {
                        group.Add(neighbor);
                    }
                }
            }
            return new CharacterInfluenceGroup(character, group);
        }

    }

    public class CharacterInfluenceGroup
    {
        public Character Character { get; }
        public List<Cell> InfluencedCells { get; }

        public CharacterInfluenceGroup(Character character, List<Cell> influencedCells)
        {
            Character = character;
            InfluencedCells = influencedCells;
        }
    }
    
}