using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Model {
    [CreateAssetMenu(fileName = "TetrisResources", menuName = "ScriptableObjects/TetrisResources", order = 1)]
    public class TetrisResources : ScriptableObject
    {
        public event Action OnDataChanged;
        private Tetri draggedTetri; // 保存当前被拖动的Tetri

        [SerializeField]
        private List<Tetri> tetriList = new List<Tetri>(); // 管理Tetri列表

        [SerializeField]
        private List<Tetri> usedTetriList = new List<Tetri>(); // 已使用的Tetri列表

        [SerializeField]
        private List<Tetri> unusedTetriList = new List<Tetri>(); // 未使用的Tetri列表

        [SerializeField]
        private List<Tetri> unusedTetriListTemplate = new List<Tetri>(); // 未使用的Tetri列表模板


        public void SetTetriDragged(Tetri tetri)
        {
            draggedTetri = tetri;
        }

        public Tetri GetDraggedTetri()
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
            List<Tetri> drawnTetri = unusedTetriList.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

            // 从unusedTetriList中移除抽取的Tetri，并添加到tetriList中
            foreach (var tetri in drawnTetri)
            {
                unusedTetriList.Remove(tetri);
                tetriList.Add(tetri);
            }

            OnDataChanged?.Invoke();
        }

        public void AddUnusedTetri(Tetri tetri)
        {
            unusedTetriList.Add(tetri);
            OnDataChanged?.Invoke();
        }

        public void AddUnusedTetriRange(List<Tetri> tetriRange)
        {
            unusedTetriList.AddRange(tetriRange);
            OnDataChanged?.Invoke();
        }

        
        public void UseTetri(Tetri tetri)
        {
            tetriList.Remove(tetri);
            usedTetriList.Add(tetri);
            OnDataChanged?.Invoke(); // 触发数据变化事件
        }

        public List<Tetri> GetAllTetris()
        {
            return new List<Tetri>(tetriList);
        }

        public bool IsEmpty()
        {
            return tetriList == null || tetriList.Count == 0;
        }

        internal void Reset()
        {
            tetriList.Clear();
            usedTetriList.Clear();
            unusedTetriList.Clear();
            // 将模板中的数据添加到unusedTetriList中
            unusedTetriList.AddRange(unusedTetriListTemplate.Select(t => (Tetri)t.Clone()).ToList());

            OnDataChanged?.Invoke(); // 触发数据变化事件
        }
    }
}
