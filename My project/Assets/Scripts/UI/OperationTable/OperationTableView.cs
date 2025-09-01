using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace View
{
    public class OperationTableView : MonoBehaviour
    {
        public event Action<Operation.Tetri> OnItemCreated;
        [SerializeField] private GameObject backgroundGridPrefab;
        [SerializeField] private GameObject TetriPrefab;

        [SerializeField] private Transform backgroundRoot;
        [SerializeField] private Transform placedTetrisRoot;
        [SerializeField] private float cellSize;
        [SerializeField] private int width;
        [SerializeField] private int height;
        public void Initialize()
        {
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

        public void Refresh(IReadOnlyList<Model.PlacedTetri> placedTetris)
        {
            foreach (Transform child in placedTetrisRoot)
            {
                Destroy(child.gameObject);
            }

            Vector2 leftTopLocal = new Vector2(-width / 2f + 0.5f, height / 2f - 0.5f);

            foreach (var placed in placedTetris)
            {

                GameObject itemObj = Instantiate(TetriPrefab, placedTetrisRoot);
                var tetriComponent = itemObj.GetComponent<Operation.Tetri>();
                tetriComponent.Initialize(placed.Tetri);

                float x = leftTopLocal.x + (placed.Position.x + 1.5f) * cellSize;
                float y = leftTopLocal.y - (placed.Position.y + 1.5f) * cellSize;
                itemObj.transform.localPosition = new Vector3(x, y, 0);
                OnItemCreated?.Invoke(tetriComponent);

            }

        }
    }
}