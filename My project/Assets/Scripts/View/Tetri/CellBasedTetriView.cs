using Model.Tetri;
using UnityEngine;
using TetriModel = Model.Tetri.Tetri;

namespace View.Tetri
{
    public class CellBasedTetriView : TetriView<CellBasedTetri>
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellsRoot;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cellPrefab == null)
            {
                Debug.LogError($"[{nameof(CellBasedTetriView)}] Cell prefab is missing.", this);
            }
            else if (cellPrefab.GetComponent<View.OperationTable.CellView>() == null)
            {
                Debug.LogError($"[{nameof(CellBasedTetriView)}] Cell prefab must have a CellView component.", this);
            }

            if (cellsRoot == null)
            {
                Debug.LogError($"[{nameof(CellBasedTetriView)}] Cells root is missing.", this);
            }
        }
#endif

        protected override void RebuildFromModel()
        {
            if (Model == null) return;

            ClearCells();

            var cellViews = new View.OperationTable.CellView[TetriModel.TetriGridSize, TetriModel.TetriGridSize];

            foreach (var entry in Model.Cells)
            {
                var tetriGridPosition = entry.Key;
                var cellObject = Instantiate(cellPrefab, cellsRoot);
                cellObject.transform.localPosition = TetriGridLayout.GetCellCenterInTetriSpace(tetriGridPosition);

                var cellView = cellObject.GetComponent<View.OperationTable.CellView>();
                cellView.Init(entry.Value);
                cellViews[tetriGridPosition.x, tetriGridPosition.y] = cellView;
            }

            // 设置边框
            for (int horizontal = 0; horizontal < TetriModel.TetriGridSize; horizontal++)
            {
                for (int vertical = 0; vertical < TetriModel.TetriGridSize; vertical++)
                {
                    var cellView = cellViews[horizontal, vertical];
                    if (cellView == null) continue;

                    bool top = (vertical == 0) || (cellViews[horizontal, vertical - 1] == null);
                    bool bottom = (vertical == TetriModel.TetriGridSize - 1) || (cellViews[horizontal, vertical + 1] == null);
                    bool left = (horizontal == 0) || (cellViews[horizontal - 1, vertical] == null);
                    bool right = (horizontal == TetriModel.TetriGridSize - 1) || (cellViews[horizontal + 1, vertical] == null);

                    cellView.SetBorderVisibility(top, bottom, left, right);
                }
            }
        }

        private void ClearCells()
        {
            for (int i = cellsRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(cellsRoot.GetChild(i).gameObject);
            }
        }
    }
}