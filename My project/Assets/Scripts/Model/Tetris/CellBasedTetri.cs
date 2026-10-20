using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Model.Resonance;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public sealed class CellBasedTetri : Tetri
    {
        private readonly Dictionary<Vector2Int, Cell> cells;

        public override TetriType Type => TetriType.CellBased;
        public IReadOnlyDictionary<Vector2Int, Cell> Cells { get; }
        public Cell MainCell { get; private set; }

        public CellBasedTetri(IReadOnlyDictionary<Vector2Int, Cell> cells, Cell mainCell, int upgradedTimes = 0)
            : base(upgradedTimes)
        {
            this.cells = new Dictionary<Vector2Int, Cell>(cells);
            Cells = new ReadOnlyDictionary<Vector2Int, Cell>(this.cells);
            MainCell = mainCell;
        }

        public override IReadOnlyCollection<Vector2Int> GetOccupiedTetriGridPositions()
        {
            return cells.Keys;
        }

        public override void Rotate()
        {
            // Remove a placed Tetri before rotating, then place it again so the table occupancy snapshot stays in sync.
            var rotatedCells = new Dictionary<Vector2Int, Cell>(cells.Count);
            foreach (var entry in cells)
            {
                rotatedCells.Add(RotateGridPosition(entry.Key), entry.Value);
            }

            cells.Clear();
            foreach (var entry in rotatedCells)
            {
                cells.Add(entry.Key, entry.Value);
            }
        }

        public Dictionary<ResonanceType, int> GetResonanceCounts()
        {
            var counts = new Dictionary<ResonanceType, int>();
            foreach (var cell in cells.Values)
            {
                var resonance = cell.Resonance;
                if (resonance == ResonanceType.None)
                {
                    continue;
                }

                counts.TryGetValue(resonance, out int count);
                counts[resonance] = count + 1;
            }
            return counts;
        }

        public void ValidateConfiguration()
        {
            if (cells.Count != 4)
            {
                throw new InvalidOperationException("A normal Tetri must contain exactly four cells.");
            }

            if (UpgradedTimes < 0 || UpgradedTimes > MaxUpgradedTimes)
            {
                throw new InvalidOperationException("Normal Tetri upgrade count is outside the allowed range.");
            }

            var uniqueCells = new HashSet<Cell>();
            int mainCellOccurrences = 0;
            foreach (var entry in cells)
            {
                var gridPosition = entry.Key;
                if (gridPosition.x < 0 || gridPosition.x >= TetriGridSize || gridPosition.y < 0 || gridPosition.y >= TetriGridSize)
                {
                    throw new InvalidOperationException($"Cell grid position {gridPosition} is outside the Tetri grid.");
                }

                var cell = entry.Value;
                if (cell == null || cell is EmptySlot)
                {
                    throw new InvalidOperationException($"Grid position {gridPosition} must contain an actual cell.");
                }

                if (!uniqueCells.Add(cell))
                {
                    throw new InvalidOperationException("Each occupied position must contain a distinct cell instance.");
                }

                if (ReferenceEquals(cell, MainCell))
                {
                    mainCellOccurrences++;
                }
            }

            if (mainCellOccurrences != 1)
            {
                throw new InvalidOperationException("The main cell must belong to the Tetri at exactly one position.");
            }
        }

        protected override void UpgradeContent()
        {
            MainCell.Level += 1;
        }
    }
}