using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Model.Tetri;


namespace UI.TetrisResource {
    public class TetrisResourcePanel : MonoBehaviour
    {
        public event Action<Tetri> OnTetriClicked;
        public event Action<TetrisResourceItem> OnTetriResourceItemBeginDrag;
        [SerializeField] private GameObject tetriResourceItemPrefab; // 用于显示Tetri的预制件

        [SerializeField] private Transform usablePanelTransform;  // Panel for available Tetris pieces
        [SerializeField] private Transform usedPanelTransform;    // Panel for used Tetris pieces
        [SerializeField] private Transform unusedPanelTransform;  // Panel for unused/locked Tetris pieces
    

        private List<TetrisResourceItem> itemList = new List<TetrisResourceItem>();


        public void UpdatePanels(List<Tetri> usableTetriList, List<Tetri> usedTetriList, List<Tetri> unusedTetriList)
        {
            // Clear all panels
            ClearPanel(usablePanelTransform);
            ClearPanel(usedPanelTransform);
            ClearPanel(unusedPanelTransform);

            // Clear item tracking list
            itemList.Clear();

            // Update usable panel
            foreach (var tetri in usableTetriList)
            {
                TetrisResourceItem resourceItem = TetrisResourceItemFactory.CreateInstance(tetriResourceItemPrefab, usablePanelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked;
                resourceItem.OnItemBeginDrag += HandleItemBeginDrag;
                itemList.Add(resourceItem);
            }

            // Update used panel
            foreach (var tetri in usedTetriList)
            {
                TetrisResourceItem resourceItem = TetrisResourceItemFactory.CreateInstance(tetriResourceItemPrefab, usedPanelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked;
            }

            // Update unused panel
            foreach (var tetri in unusedTetriList)
            {
                TetrisResourceItem resourceItem = TetrisResourceItemFactory.CreateInstance(tetriResourceItemPrefab, unusedPanelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked;
            }
        }

        private void ClearPanel(Transform panel)
        {
            if (panel == null) return;

            foreach (Transform child in panel)
            {
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
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
            OnTetriResourceItemBeginDrag?.Invoke(itemList[index]);
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
