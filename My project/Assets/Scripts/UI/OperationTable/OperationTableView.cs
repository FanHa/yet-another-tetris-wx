using System;
using System.Collections.Generic;
using Model;
using Model.Tetri;
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
        [SerializeField] private Operation.TetriFactory tetriFactory;
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

        public void Refresh(IReadOnlyDictionary<Model.Tetri.Tetri, Vector2Int> placedTetris)
        {
            foreach (Transform child in placedTetrisRoot)
            {
                Destroy(child.gameObject);
            }

            Vector2 leftTopLocal = new Vector2(-width / 2f + 0.5f, height / 2f - 0.5f);

            foreach (var placed in placedTetris)
            {
                // todo 这里调用工厂的方法来创建
                Tetri tetri = placed.Key;
                Vector2Int position = placed.Value;
                Operation.Tetri tetriComponent = tetriFactory.CreateTetri(tetri);
                tetriComponent.transform.SetParent(placedTetrisRoot, false);
                tetriComponent.Initialize(tetri);

                float x = leftTopLocal.x + (position.x + 1.5f) * cellSize;
                float y = leftTopLocal.y - (position.y + 1.5f) * cellSize;
                tetriComponent.transform.localPosition = new Vector3(x, y, 0);
                OnItemCreated?.Invoke(tetriComponent);

            }

        }
    }
}