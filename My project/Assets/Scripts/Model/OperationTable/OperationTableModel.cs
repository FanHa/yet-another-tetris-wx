using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    public sealed class OperationTableModel
    {
        public int Width { get; }
        public int Height { get; }
        public event Action OnTableChanged;

        private Dictionary<Model.Tetri.Tetri, Vector2Int> placedGridPositionByTetri = new();
        public IReadOnlyDictionary<Model.Tetri.Tetri, Vector2Int> PlacedGridPositionByTetri => placedGridPositionByTetri;

        private OccupiedSlot[,] occupiedGrid;


        public OperationTableModel(int width, int height)
        {
            Width = width;
            Height = height;
            occupiedGrid = new OccupiedSlot[width, height];
        }

        /// <summary>
        /// 尝试放置一个 Tetri 到指定位置
        /// </summary>
        /// <param name="tetri">要放置的 Tetri</param>
        /// <param name="topLeftGridPosition">Tetri 左上角对应的操作台格坐标</param>
        /// <returns>是否放置成功</returns>
        public bool TryPlaceTetri(Model.Tetri.Tetri tetri, Vector2Int topLeftGridPosition)
        {
            if (tetri == null) return false;
            if (placedGridPositionByTetri.ContainsKey(tetri)) return false;

            var occupiedTetriGridPositions = tetri.GetOccupiedTetriGridPositions();
            // 边界 & 占用检测
            foreach (var tetriCellGridPosition in occupiedTetriGridPositions)
            {
                var tableGridPosition = topLeftGridPosition + tetriCellGridPosition;
                if (tableGridPosition.x < 0 || tableGridPosition.x >= Width || tableGridPosition.y < 0 || tableGridPosition.y >= Height) return false;
                if (occupiedGrid[tableGridPosition.x, tableGridPosition.y] != null) return false;
            }
            tetri.OnDataChanged += HandleTetriChanged;

            // 写入
            foreach (var tetriCellGridPosition in occupiedTetriGridPositions)
            {
                var tableGridPosition = topLeftGridPosition + tetriCellGridPosition;
                occupiedGrid[tableGridPosition.x, tableGridPosition.y] = new OccupiedSlot
                {
                    OwnerTetri = tetri,
                    TetriCellGridPosition = tetriCellGridPosition
                };
            }

            placedGridPositionByTetri[tetri] = topLeftGridPosition;
            OnTableChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            foreach (KeyValuePair<Tetri.Tetri, Vector2Int> kv in placedGridPositionByTetri)
            {
                kv.Key.OnDataChanged -= HandleTetriChanged;
            }
            placedGridPositionByTetri.Clear();
            occupiedGrid = new OccupiedSlot[Width, Height];
            OnTableChanged?.Invoke();
        }

        public void RemoveTetri(Model.Tetri.Tetri tetri)
        {
            if (tetri == null) return;
            if (!placedGridPositionByTetri.TryGetValue(tetri, out var topLeftGridPosition)) return;
            tetri.OnDataChanged -= HandleTetriChanged;
            foreach (var tetriCellGridPosition in tetri.GetOccupiedTetriGridPositions())
            {
                var tableGridPosition = topLeftGridPosition + tetriCellGridPosition;
                if (tableGridPosition.x < 0 || tableGridPosition.x >= Width || tableGridPosition.y < 0 || tableGridPosition.y >= Height) continue;
                occupiedGrid[tableGridPosition.x, tableGridPosition.y] = null;
            }
            placedGridPositionByTetri.Remove(tetri);
            OnTableChanged?.Invoke();

        }

        private void HandleTetriChanged()
        {
            // Tetri 变动时，通知外部刷新
            OnTableChanged?.Invoke();
        }

        public List<CharacterBuild> GetCharacterBuilds()
        {
            var result = new List<CharacterBuild>();
            foreach (var placedTetri in placedGridPositionByTetri)
            {
                if (placedTetri.Key is CharacterTetri characterTetri)
                {
                    result.Add(CreateCharacterBuild(characterTetri, placedTetri.Value));
                }
            }

            return result;
        }

        public CharacterBuild GetPlacedCharacterBuild(CharacterTetri tetri)
        {
            if (!placedGridPositionByTetri.TryGetValue(tetri, out var topLeftGridPosition)) return null;

            return CreateCharacterBuild(tetri, topLeftGridPosition);
        }

        private CharacterBuild CreateCharacterBuild(CharacterTetri tetri, Vector2Int topLeftGridPosition)
        {
            var characterAnchorGridPosition = topLeftGridPosition + tetri.InfluenceAnchor;
            var influencedCells = new List<Cell>();
            foreach (var influenceGridOffset in tetri.Character.GetInfluenceOffsets())
            {
                var tableGridPosition = characterAnchorGridPosition + influenceGridOffset;
                if (tableGridPosition.x < 0 || tableGridPosition.x >= Width || tableGridPosition.y < 0 || tableGridPosition.y >= Height)
                {
                    continue;
                }

                var slot = occupiedGrid[tableGridPosition.x, tableGridPosition.y];
                if (slot?.OwnerTetri is not CellBasedTetri cellBasedTetri)
                {
                    continue;
                }

                var cell = cellBasedTetri.Cells[slot.TetriCellGridPosition];
                if (!influencedCells.Contains(cell))
                {
                    influencedCells.Add(cell);
                }
            }

            return new CharacterBuild(tetri.Character, influencedCells, tetri);
        }

    }
    
    [Serializable]
    public class OccupiedSlot
    {
        public Model.Tetri.Tetri OwnerTetri;
        public Vector2Int TetriCellGridPosition;   // 格子在 Tetri 内的二维索引
    }
}