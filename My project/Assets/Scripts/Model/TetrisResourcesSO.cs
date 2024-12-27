using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [CreateAssetMenu(fileName = "TetrisResourcesSO", menuName = "ScriptableObjects/TetrisResourcesSO", order = 1)]
    public class TetrisResourcesSO : ScriptableObject
    {
        public event Action OnDataChanged;
        private Tetri draggedTetri; // 保存当前被拖动的Tetri

        [SerializeField]
        private List<Tetri> tetriList = new List<Tetri>(); // 管理Tetri列表

        public void SetTetriDragged(Tetri tetri)
        {
            draggedTetri = tetri;
            Debug.Log($"Tetri set as dragged: {tetri}");
        }

        public Tetri GetDraggedTetri()
        {
            return draggedTetri;
        }

        public void AddTetri(Tetri tetri)
        {
            tetriList.Add(tetri);
            Debug.Log($"Tetri added: {tetri}");
            OnDataChanged?.Invoke(); // 触发数据变化事件
        }

        public void AddTetriRange(List<Tetri> tetriRange)
        {
            tetriList.AddRange(tetriRange);
            Debug.Log($"Tetri range added: {tetriRange.Count} items");
            OnDataChanged?.Invoke(); // 触发数据变化事件
        }
        

        public void RemoveTetri(Tetri tetri)
        {
            tetriList.Remove(tetri);
            Debug.Log($"Tetri removed: {tetri}");
        }

        public List<Tetri> GetAllTetris()
        {
            return new List<Tetri>(tetriList);
        }

        // 其他方法和属性
    }
}
