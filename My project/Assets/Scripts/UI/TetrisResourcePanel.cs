using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Tetri = Model.Tetri;


namespace UI {
    public class TetrisResourcePanel : MonoBehaviour
    {
        public event Action<Tetri> OnTetriClicked;
        public event Action<Tetri> OnTetriBeginDrag;
        [SerializeField] private GameObject tetriResourceItemPrefab; // 用于显示Tetri的预制件
        [SerializeField] private Transform panelTransform; // 用于显示Tetri的面板

        private List<TetrisResourceItem> itemList = new List<TetrisResourceItem>();

        // public void Init(List<Tetri> initialTetriList)
        // {
        //     tetriList = initialTetriList;
        //     UpdatePanel();
        // }

        // public void AddTetri(Tetri tetri)
        // {
        //     tetriList.Add(tetri);
        //     UpdatePanel();
        // }

        // public void AddTetriRange(List<Tetri> tetriRange)
        // {
        //     tetriList.AddRange(tetriRange);
        //     UpdatePanel();
        // }

        // public void RemoveTetri(Tetri tetri)
        // {
        //     tetriList.Remove(tetri);
        //     UpdatePanel();
        // }

        public void UpdatePanel(List<Tetri> tetriList)
        {
            // 清空面板
            foreach (Transform child in panelTransform)
            {
                Destroy(child.gameObject);
            }

            // 按顺序显示Tetri
            foreach (var tetri in tetriList)
            {
                TetrisResourceItem resourceItem = TetrisResourceItem.CreateInstance(tetriResourceItemPrefab, panelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked; // 添加点击事件
                resourceItem.OnItemBeginDrag += HandleItemBeginDrag; // 添加开始拖动事件
                itemList.Add(resourceItem);
            }
        }

        private void HandleItemBeginDrag(TetrisResourceItem item)
        {
            int index = itemList.IndexOf(item);
            if (index == -1)
            {
                // ResetDraggedItem();
                return;
            }
            OnTetriBeginDrag?.Invoke(itemList[index].GetTetri());
        }

        private void ResetDraggedItem()
        {
            throw new NotImplementedException();
        }

        private void HandleItemClicked(TetrisResourceItem item)
        {
            // int index = item.transform.GetSiblingIndex();
            // Tetri tetri = tetriList[index];
            // OnTetriClicked?.Invoke(tetri);
        }
    }
}
