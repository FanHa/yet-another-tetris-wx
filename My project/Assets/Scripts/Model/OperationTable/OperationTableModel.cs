using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTableModel", menuName = "Tetris/OperationTableModel")]
    public class OperationTableModel : ScriptableObject
    {
        public event Action OnChanged;
        [System.Serializable]
        public class PlacedTetri
        {
            public Model.Tetri.Tetri tetri;
            public Vector2Int position; // 左上角在表格上的格子坐标
        }

        [SerializeField]
        private List<PlacedTetri> placedTetris = new List<PlacedTetri>();

        public IReadOnlyList<PlacedTetri> PlacedTetris => placedTetris;

        private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();


        /// <summary>
        /// 尝试放置一个 Tetri 到指定位置
        /// </summary>
        /// <param name="tetri">要放置的 Tetri</param>
        /// <param name="position">左上角在表格上的格子坐标</param>
        /// <param name="occupiedCells">Tetri 相对左上角的所有占用格子</param>
        /// <returns>是否放置成功</returns>
        public bool TryPlaceTetri(Model.Tetri.Tetri tetri, Vector2Int position)
        {
            var newCells = new List<Vector2Int>();
            List<Vector2Int> tetriOccupiedPositions = tetri.GetOccupiedPositions();
            foreach (var cell in tetriOccupiedPositions)
                newCells.Add(position + cell);

            // 只需判断一次
            foreach (var cell in newCells)
            {
                if (cell.x < 0 || cell.x > 9 || cell.y < 0 || cell.y > 9)
                {
                    // 超出边界
                    Debug.Log($"超出边界: {cell}");
                    return false;
                }
                if (occupiedCells.Contains(cell))
                {
                    // 已经被占用
                    Debug.Log($"已被占用: {cell}");
                    return false;
                }
            }

            var placedTetri = new PlacedTetri
            {
                tetri = tetri,
                position = position,
            };
            placedTetris.Add(placedTetri);
            foreach (var cell in newCells)
                occupiedCells.Add(cell);
            OnChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            placedTetris.Clear();
            occupiedCells.Clear();
            OnChanged?.Invoke();
        }

        public void RemoveTetri(PlacedTetri placedTetri)
        {
            if (placedTetris.Contains(placedTetri))
            {
                // 移除占用的格子
                List<Vector2Int> tetriOccupiedPositions = placedTetri.tetri.GetOccupiedPositions();
                foreach (var cell in tetriOccupiedPositions)
                {
                    occupiedCells.Remove(placedTetri.position + cell);
                }

                // 从列表中移除
                placedTetris.Remove(placedTetri);

                // 触发更新事件
                OnChanged?.Invoke();
            }
        }

    }
}