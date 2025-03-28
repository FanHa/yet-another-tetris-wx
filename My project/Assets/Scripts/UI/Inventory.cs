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
        private Dictionary<int, Model.InventoryItem> inventoryData; // Store inventory data locally

        private void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }

        public void UpdateData(Dictionary<int, Model.InventoryItem> inventoryState)
        {
            inventoryData = inventoryState; // Save the data locally

            // Clear excess UI elements
            while (items.Count > inventoryState.Count)
            {
                Destroy(items[items.Count - 1].gameObject);
                items.RemoveAt(items.Count - 1);
            }

            // Update or create UI elements
            int index = 0;
            foreach (var kvp in inventoryState)
            {
                if (index < items.Count)
                {
                    items[index].SetData(kvp.Value.UnitSprite);
                }
                else
                {
                    InventoryItem item = Instantiate(itemPrefab, contentPanel);
                    item.SetData(kvp.Value.UnitSprite);
                    items.Add(item);
                    item.OnItemClicked += HandleItemSelection;
                    item.OnItemDroppedOn += HandleSwap;
                    item.OnItemBeginDrag += HandleBeginDrag;
                    item.OnItemEndDrag += HandleEndDrag;
                    item.OnRightMouseBtnClick += HandleShowItemActions;
                }
                index++;
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

            if (inventoryData.TryGetValue(index, out var selectedItem))
            {
                itemDescription.SetDescription(selectedItem); // Directly set the description
                DeselectAllItems();
                items[index].Select();
                OnDescriptionRequested?.Invoke(index);
            }
            else
            {
                Debug.LogError("Item data not found");
            }
        }

        public void Show()
        {
            // todo 很奇怪的问题，这里必须调用两次在能在第一次show时显示出来，不然就不行
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

        public void ResetAllItems()
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
        }
    }
}
