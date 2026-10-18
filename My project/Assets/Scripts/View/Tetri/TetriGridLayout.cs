using Model.Tetri;
using UnityEngine;
using TetriModel = Model.Tetri.Tetri;

namespace View.Tetri
{
    public static class TetriGridLayout
    {
        public static float CenterOffset => (TetriModel.TetriGridSize - 1) * 0.5f;

        public static Vector3 GetCellCenterInTetriSpace(Vector2Int tetriGridPosition)
        {
            float centerOffset = CenterOffset;
            return new Vector3(
                tetriGridPosition.x - centerOffset,
                -tetriGridPosition.y + centerOffset,
                0f);
        }
    }
}