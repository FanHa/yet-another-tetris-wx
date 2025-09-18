using Model.Tetri;
using UnityEngine;

namespace Operation
{
    public class TetriPiece : Tetri
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellsRoot;
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;

        protected override void RebuildFromModel()
        {
            if (ModelTetri == null) return;

            ClearCells();

            var shape = ModelTetri.Shape;
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            var cellObjs = new Operation.Cell[rows, cols];

            float offsetX = (rows - 1) / 2f;
            float offsetY = (cols - 1) / 2f;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var cell = shape[i, j];
                    if (cell is not Model.Tetri.Empty)
                    {
                        GameObject cellObj = Instantiate(cellPrefab, this.cellsRoot);
                        float localX = i - offsetX;
                        float localY = -j + offsetY;
                        cellObj.transform.localPosition = new Vector3(localX, localY, 0);

                        var cellComp = cellObj.GetComponent<Operation.Cell>();
                        cellComp.Init(cell);
                        cellObjs[i, j] = cellComp;
                    }
                }
            }

            // 设置边框
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var cellComp = cellObjs[i, j];
                    if (cellComp == null) continue;

                    bool top = (j == 0) || (cellObjs[i, j - 1] == null);
                    bool bottom = (j == cols - 1) || (cellObjs[i, j + 1] == null);
                    bool left = (i == 0) || (cellObjs[i - 1, j] == null);
                    bool right = (i == rows - 1) || (cellObjs[i + 1, j] == null);

                    cellComp.SetBorderVisibility(top, bottom, left, right);
                }
            }
        }

        private void ClearCells()
        {
            if (cellsRoot == null) return;
            for (int i = cellsRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(cellsRoot.GetChild(i).gameObject);
            }
        }
    }
}