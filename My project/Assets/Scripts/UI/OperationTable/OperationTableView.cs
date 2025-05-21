using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace View
{
    public class OperationTableView : MonoBehaviour
    {
        [Header("背景格子预制件")]
        [SerializeField] private GameObject backgroundGridPrefab;
        [SerializeField] private GameObject TetriPrefab;

        [SerializeField] private Transform backgroundRoot;
        [SerializeField] private Transform placedTetrisRoot;
        [SerializeField] private float cellSize;
        [SerializeField] private int width;
        [SerializeField] private int height;
        public void CreateGridBackground()
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

        public void Refresh(IReadOnlyList<OperationTableModel.PlacedTetri> placedTetris)
        {
            foreach (Transform child in placedTetrisRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var placed in placedTetris)
            {

                GameObject itemObj = Instantiate(TetriPrefab, placedTetrisRoot);
                var tetriComponent = itemObj.GetComponent<Operation.Tetri>();
                tetriComponent.Initialize(placed.tetri);
                
                // 计算偏移后的位置
                float offsetX = -5 * cellSize + (cellSize / 2); // 向左偏移 5 个单位
                float offsetY = 5 * cellSize - (cellSize / 2);  // 向上偏移 5 个单位
                itemObj.transform.localPosition = new Vector3(
                    placed.position.x * cellSize + offsetX,
                    -placed.position.y * cellSize + offsetY,
                    0
                );

            }

        }
    }
}