using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using OperationTableModel = Model.OperationTableModel;

namespace View.OperationTable
{
    public class OperationTableView : MonoBehaviour
    {
        public event Action<View.Tetri.TetriView> OnItemCreated;
        [SerializeField] private GameObject backgroundGridPrefab;

        [SerializeField] private Transform backgroundRoot;
        [SerializeField] private Transform placedTetrisRoot;
        private OperationTableGridLayout gridLayout;
        [SerializeField] private View.Tetri.TetriViewFactory tetriViewFactory;

        private readonly Dictionary<Model.Tetri.Tetri, View.Tetri.TetriView> tetriObjectMap = new();

        public void Initialize(OperationTableGridLayout gridLayout)
        {
            this.gridLayout = gridLayout;

            for (int y = 0; y < gridLayout.Height; y++)
            {
                for (int x = 0; x < gridLayout.Width; x++)
                {
                    var tableGridPosition = new Vector2Int(x, y);
                    GameObject cell = Instantiate(backgroundGridPrefab, backgroundRoot);
                    cell.transform.localPosition = gridLayout.GetTableCellCenterInTableSpace(tableGridPosition);
                    cell.name = $"Cell_{x}_{y}";
                }
            }
        }

        public void Refresh(OperationTableModel operationTableModel)
        {
            tetriObjectMap.Clear();
            foreach (Transform child in placedTetrisRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var placed in operationTableModel.PlacedGridPositionByTetri)
            {
                Model.Tetri.Tetri tetri = placed.Key;
                Vector2Int topLeftGridPosition = placed.Value;
                View.Tetri.TetriView tetriComponent = tetriViewFactory.CreateTetri(tetri);
                tetriComponent.transform.SetParent(placedTetrisRoot, false);
                tetriComponent.transform.localPosition = gridLayout.GetTetriCenterInTableSpace(topLeftGridPosition);

                tetriObjectMap[tetri] = tetriComponent;
                OnItemCreated?.Invoke(tetriComponent);

            }

        }

        public Vector3 GetTetriWorldPosition(Model.Tetri.Tetri tetri)
        {
            return tetriObjectMap[tetri].transform.position;
        }

    }
}