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

            OnDataChanged?.Invoke();
        }

        public void AddUnusedTetri(Tetri.Tetri tetri)
        {
            unusedTetriList.Add(tetri);
            OnDataChanged?.Invoke();
        }

        public void AddUnusedTetriRange(List<Tetri.Tetri> tetriRange)
        {
            unusedTetriList.AddRange(tetriRange);
            OnDataChanged?.Invoke();
        }

        public void AddUsableTetri(Tetri.Tetri tetri)
        {
            tetriList.Add(tetri);
            OnDataChanged?.Invoke(); // 触发数据变化事件
        }

        
        public void UseTetri(Tetri.Tetri tetri)
        {
            tetriList.Remove(tetri);
            usedTetriList.Add(tetri);
            OnDataChanged?.Invoke(); // 触发数据变化事件
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
            OnDataChanged?.Invoke(); // 触发数据变化事件
        }

        public void InitialUnusedTetris(List<Tetri.Tetri> tetris)
        {
            unusedTetriList.AddRange(tetris);
            OnDataChanged?.Invoke(); // 触发数据变化事件
        }
    }
}
