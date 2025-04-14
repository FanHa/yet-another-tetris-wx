using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Model.Tetri;



namespace Model {
    [CreateAssetMenu(fileName = "TetrisResources", menuName = "ScriptableObjects/TetrisResources", order = 1)]
    public class TetrisResources : ScriptableObject
    {
        public event Action OnDataChanged;
        private Tetri.Tetri draggedTetri; // 保存当前被拖动的Tetri

        [SerializeField]
        private List<Tetri.Tetri> tetriList = new List<Tetri.Tetri>(); // 管理Tetri列表
        [SerializeField]
        private List<Tetri.Tetri> usedTetriList = new List<Tetri.Tetri>(); // 已使用的Tetri列表
        [SerializeField]
        private List<Tetri.Tetri> unusedTetriList = new List<Tetri.Tetri>(); // 未使用的Tetri列表
        // 用于跟踪当前存在的 Cell 类型
        private HashSet<Type> cellTypes = new HashSet<Type>();

        public IReadOnlyCollection<Type> CellTypes => cellTypes; // 只读访问

        public void SetTetriDragged(Tetri.Tetri tetri)
        {
            draggedTetri = tetri;
        }

        public Tetri.Tetri GetDraggedTetri()
        {
            return draggedTetri;
        }

        public void DrawRandomTetriFromUnusedList(int count)
        {
            if (unusedTetriList.Count < count)
            {
                // 如果unusedTetriList为空，将usedTetriList中的所有Tetri移动到unusedTetriList中
                unusedTetriList.AddRange(usedTetriList);
                usedTetriList.Clear();
            }

            // 随机抽取若干个Tetri
            List<Tetri.Tetri> drawnTetri = unusedTetriList.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

            // 从unusedTetriList中移除抽取的Tetri，并添加到tetriList中
            foreach (var tetri in drawnTetri)
            {
                unusedTetriList.Remove(tetri);
                tetriList.Add(tetri);
            }

            RecalculateCellTypes();
        }

        public void AddUnusedTetri(Tetri.Tetri tetri)
        {
            unusedTetriList.Add(tetri);
            RecalculateCellTypes();
        }

        public void AddUnusedTetriRange(List<Tetri.Tetri> tetriRange)
        {
            unusedTetriList.AddRange(tetriRange);
            RecalculateCellTypes();
        }

        public void AddUsableTetri(Tetri.Tetri tetri)
        {
            tetriList.Add(tetri);
            RecalculateCellTypes();
        }

        
        public void UseTetri(Tetri.Tetri tetri)
        {
            tetriList.Remove(tetri);
            if (!tetri.IsDisposable)
            {
                usedTetriList.Add(tetri);
            }
            RecalculateCellTypes();
        }

        public List<Tetri.Tetri> GetUsableTetris()
        {
            return new List<Tetri.Tetri>(tetriList);
        }

        public List<Tetri.Tetri> GetUsedTetris()
        {
            return new List<Tetri.Tetri>(usedTetriList);
        }

        public List<Tetri.Tetri> GetUnusedTetris()
        {
            return new List<Tetri.Tetri>(unusedTetriList);
        }

        public bool IsEmpty()
        {
            return tetriList == null || tetriList.Count == 0;
        }

        public void Reset()
        {
            tetriList.Clear();
            usedTetriList.Clear();
            unusedTetriList.Clear();
            RecalculateCellTypes();
        }

        public void InitialUnusedTetris(List<Tetri.Tetri> tetris)
        {
            unusedTetriList.AddRange(tetris);
            RecalculateCellTypes();
        }

        private void RecalculateCellTypes()
        {
            cellTypes.Clear(); // 清空当前的 cellTypes

            // 遍历所有 Tetri 列表，重新计算包含的 Cell 类型
            foreach (var tetri in tetriList.Concat(unusedTetriList).Concat(usedTetriList))
            {
                foreach (var position in tetri.GetOccupiedPositions())
                {
                    Cell cell = tetri.Shape[position.x, position.y];
                    cellTypes.Add(cell.GetType());
                }
            }

            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }
    }
}
