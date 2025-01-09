using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public class Inventory : MonoBehaviour
    {
        public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems;
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private Transform contentPanel;
        [SerializeField] private InventoryDescription itemDescription;
        [SerializeField] private InventoryMouseFollower mouseFollower;
        List<InventoryItem> items = new List<InventoryItem>();
        private int currentlyDraggedItemIndex = -1;

        private void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }
        public void InitializeInventory(int size)
        {
            for (int i = 0; i < size; i++)
            {
                InventoryItem item = Instantiate(itemPrefab, contentPanel);
                items.Add(item);
                item.OnItemClicked += HandleItemSelection;
                item.OnItemDroppedOn += HandleSwap;
                item.OnItemBeginDrag += HandleBeginDrag;
                item.OnItemEndDrag += HandleEndDrag;
                item.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        public void UpdateData(int itemIndex, Sprite itemSprite)
        {
            if (items.Count > itemIndex)
            {
                items[itemIndex].SetData(itemSprite);
            }
        }

        private void HandleShowItemActions(InventoryItem item)
        {
            
        }

        private void HandleEndDrag(InventoryItem item)
        {
            ResetDraggedItem();
        }

        private void HandleSwap(InventoryItem item)
        {
            int index = items.IndexOf(item);
            if (index == -1)
            {
                ResetDraggedItem();
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(item);
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        private void HandleBeginDrag(InventoryItem item)
        {
            int index = items.IndexOf(item);
            if (index == -1)
            {
                Debug.LogError("Item not found");
                return;
            }
            currentlyDraggedItemIndex = index;
            HandleItemSelection(item);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite)
        {
            mouseFollower.SetData(sprite);
            mouseFollower.Toggle(true);
        }
        private void HandleItemSelection(InventoryItem item)
        {
            int index = items.IndexOf(item);
            if(index == -1)
            {
                Debug.LogError("Item not found");
                return;
            }
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show()
        {
            // todo 很奇怪的问题,这里必须SetActive两次在第一次调用后就成功将activeSelf设置为true
            gameObject.SetActive(true);
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        private void DeselectAllItems()
        {
            foreach (var item in items)
            {
                item.Deselect();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ResetDraggedItem();
        }

        internal void UpdateDescription(int itemIndex, Sprite unitSprite, string unitName, string description)
        {
            itemDescription.SetDescription(unitSprite, unitName, description);
            DeselectAllItems();
            items[itemIndex].Select();
        }

        internal void ResetAllItems()
        {
            foreach (var item in items)
            {
                item.ResetData();
                item.Deselect();
            }
        }
    }
}
