using System;
using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;

namespace View.OperationTable
{
    public class OperationTableView : MonoBehaviour
    {
        public event Action<View.Tetri.TetriView> OnItemCreated;
        [SerializeField] private GameObject backgroundGridPrefab;
        [SerializeField] private GameObject TetriPrefab;

        [SerializeField] private Transform backgroundRoot;
        [SerializeField] private Transform placedTetrisRoot;
        private float cellSize;
        private int width;
        private int height;
        [SerializeField] private View.Tetri.TetriFactory tetriFactory;
        [SerializeField] private Transform ScreenCenterPosition;

        private readonly Dictionary<Model.Tetri.Tetri, View.Tetri.TetriView> tetriObjectMap = new();

        public void Initialize(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            Vector2 origin = new Vector2(-width / 2f + 0.5f, -height / 2f + 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = origin + new Vector2(x * cellSize, y * cellSize);
                    GameObject cell = Instantiate(backgroundGridPrefab, backgroundRoot);
                    cell.transform.localPosition = pos;
                    cell.name = $"Cell_{x}_{y}";
                }
            }
        }

        public void Refresh(IReadOnlyDictionary<Model.Tetri.Tetri, Vector2Int> placedTetris)
        {
            tetriObjectMap.Clear();
            foreach (Transform child in placedTetrisRoot)
            {
                Destroy(child.gameObject);
            }

            Vector2 leftTopLocal = new Vector2(-width / 2f + 0.5f, height / 2f - 0.5f);

            foreach (var placed in placedTetris)
            {
                Model.Tetri.Tetri tetri = placed.Key;
                Vector2Int position = placed.Value;
                View.Tetri.TetriView tetriComponent = tetriFactory.CreateTetri(tetri);
                tetriComponent.transform.SetParent(placedTetrisRoot, false);
                tetriComponent.Initialize(tetri);

                float x = leftTopLocal.x + (position.x + 1.5f) * cellSize;
                float y = leftTopLocal.y - (position.y + 1.5f) * cellSize;
                tetriComponent.transform.localPosition = new Vector3(x, y, 0);

                tetriObjectMap[tetri] = tetriComponent;
                OnItemCreated?.Invoke(tetriComponent);

            }

        }

        public Vector3 GetRelativePositionByTetri(Model.Tetri.Tetri tetri)
        {
            if (tetri != null && tetriObjectMap.TryGetValue(tetri, out var obj) && obj != null)
            {
                Vector3 worldPos = obj.transform.position;
                return ScreenCenterPosition.InverseTransformPoint(worldPos);
            }
            return Vector3.zero;
        }

    }
}