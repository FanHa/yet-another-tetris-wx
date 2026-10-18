using UnityEngine;

namespace View.OperationTable
{
    public readonly struct OperationTableGridLayout
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public OperationTableGridLayout(int width, int height, float cellSize)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
        }

        public Vector3 GetTableCellCenterInTableSpace(Vector2Int tableGridPosition)
        {
            // Grid Y increases downward, while table-local Y increases upward.
            return new Vector3(
                (tableGridPosition.x - (Width - 1) * 0.5f) * CellSize,
                ((Height - 1) * 0.5f - tableGridPosition.y) * CellSize,
                0f);
        }

        public Vector3 GetTetriCenterInTableSpace(Vector2Int tetriTopLeftGridPosition)
        {
            // The Tetri view is positioned at the center of its grid, not at its top-left grid cell.
            float gridCenterOffset = View.Tetri.TetriGridLayout.CenterOffset;
            return new Vector3(
                (tetriTopLeftGridPosition.x + gridCenterOffset - (Width - 1) * 0.5f) * CellSize,
                ((Height - 1) * 0.5f - tetriTopLeftGridPosition.y - gridCenterOffset) * CellSize,
                0f);
        }

        public Vector2Int GetNearestTetriTopLeftGridPositionFromCenterInTableSpace(Vector3 tetriCenterInTableSpace)
        {
            // Measure from the center for grid position zero, then round to the nearest placement.
            Vector3 firstTetriCenterInTableSpace = GetTetriCenterInTableSpace(Vector2Int.zero);
            int gridX = Mathf.RoundToInt((tetriCenterInTableSpace.x - firstTetriCenterInTableSpace.x) / CellSize);
            int gridY = Mathf.RoundToInt((firstTetriCenterInTableSpace.y - tetriCenterInTableSpace.y) / CellSize);
            return new Vector2Int(gridX, gridY);
        }
    }
}