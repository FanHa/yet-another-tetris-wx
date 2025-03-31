using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Model.Tetri;
using WeChatWASM;

namespace UI.Resource {
    public class Panel : MonoBehaviour
    {
        public event Action<ItemSlot> OnTetriResourceItemBeginDrag;
        [SerializeField] private GameObject tetriResourceItemPrefab; // 用于显示Tetri的预制件

        [SerializeField] private Transform usablePanelTransform;  // Panel for available Tetris pieces
        [SerializeField] private Transform usedPanelTransform;    // Panel for used Tetris pieces
        [SerializeField] private Transform unusedPanelTransform;  // Panel for unused/locked Tetris pieces
        [SerializeField] private TabsManager tabsManager;
        [SerializeField] private Controller.Tetris tetrisFactory;

        // todo 这个itemList的作用是啥?
        private List<ItemSlot> itemList = new List<ItemSlot>();

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
                ItemSlot resourceItem = CreateTetrisResourceItem(
                    usablePanelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked;
                resourceItem.OnItemBeginDrag += HandleItemBeginDrag;
                itemList.Add(resourceItem);
            }

            // Update used panel
            foreach (var tetri in usedTetriList)
            {
                ItemSlot resourceItem = CreateTetrisResourceItem(
                    usedPanelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked;
            }

            // Update unused panel
            foreach (var tetri in unusedTetriList)
            {
                ItemSlot resourceItem = CreateTetrisResourceItem(
                    unusedPanelTransform, tetri);
                resourceItem.OnItemClicked += HandleItemClicked;
            }
        }

        public ItemSlot CreateTetrisResourceItem(Transform parent, Tetri tetri)
        {
            GameObject instance = Instantiate(tetriResourceItemPrefab, parent);
            ItemSlot resourceItem = instance.GetComponent<ItemSlot>();
            if (resourceItem != null)
            {
                GameObject preview = tetrisFactory.GenerateTetriPreview(tetri);
                resourceItem.Initialize(tetri, preview);
            }
            return resourceItem;
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

        private void HandleItemBeginDrag(ItemSlot item)
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

        private void HandleItemClicked(ItemSlot item)
        {
            // int index = item.transform.GetSiblingIndex();
            // Tetri tetri = tetriList[index];
            // OnTetriClicked?.Invoke(tetri);
        }

        internal void ResetTab()
        {
            tabsManager.SwitchTab(0);
        }
    }
}
