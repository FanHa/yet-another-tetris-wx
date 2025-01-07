using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [CreateAssetMenu(fileName = "TetrisResources", menuName = "ScriptableObjects/TetrisResourcesSO", order = 1)]
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

        // public IReadOnlyList<Tetri> TetriList => tetriList;
        // public IReadOnlyList<Tetri> UsedTetriList => usedTetriList;
        // public IReadOnlyList<Tetri> UnusedTetriList => unusedTetriList;

        public void SetTetriDragged(Tetri tetri)
        {
            draggedTetri = tetri;
        }

        public Tetri GetDraggedTetri()
        {
            return draggedTetri;
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

        public void ResetAllItems()
        {
            tetriList.Clear();
            OnDataChanged?.Invoke(); // 触发数据变化事件
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

        // 其他方法和属性
    }
}
