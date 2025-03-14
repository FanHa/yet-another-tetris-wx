using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;

namespace Controller {
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private UI.Inventory inventoryUI;
        [SerializeField] private Model.Inventory inventoryData;

        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;

        private static bool isInitialized = false;
        
        private void Start()
        {
            PrepareUI();
            if (!isInitialized)
            {
                PrepareInventoryData();
                isInitialized = true;
            }
        }


        public InventoryItem GenerateInventoryItemFromTetriCells(List<TetriCell> tetriCells)
        {
            Dictionary<Type, int> cellTypeCounts = new Dictionary<Type, int>();

            // 统计不同TetriCell衍生类型的数量
            foreach (var cell in tetriCells)
            {
                Type cellType = cell.GetType();
                if (cellTypeCounts.ContainsKey(cellType))
                {
                    cellTypeCounts[cellType]++;
                }
                else
                {
                    cellTypeCounts[cellType] = 1;
                }
            }

            // 找到数量最多的类型
            Type mostCommonCellType = null;
            int maxCount = 0;
            foreach (var kvp in cellTypeCounts)
            {
                if (kvp.Value > maxCount)
                {
                    mostCommonCellType = kvp.Key;
                    maxCount = kvp.Value;
                }
            }

            // 根据最多的类型生成一个InventoryItem
            string unitName = $"Generated Unit ({mostCommonCellType.Name})";
            Sprite unitSprite = cellTypeResourceMapping.GetSprite(mostCommonCellType); // 根据需要设置
            GameObject prefab = cellTypeResourceMapping.GetPrefab(mostCommonCellType); // 根据需要设置
            string description = $"Generated from {maxCount} {mostCommonCellType.Name} cells";
            int spawnInterval = 0; // 根据需要设置

            return new InventoryItem(unitName, unitSprite, prefab, description, spawnInterval, tetriCells);
        }



        private void PrepareUI()
        {
            inventoryUI.InitializeInventory(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleStartDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }
        private void PrepareInventoryData()
        {
            inventoryData.OnInventoryChanged += UpdateInventoryUI;
        }
        private void UpdateInventoryUI(Dictionary<int, Model.InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.UnitSprite);
            }
        }
        private void HandleItemActionRequest(int itemIndex)
        {
            throw new NotImplementedException();
        }
        private void HandleStartDragging(int itemIndex)
        {
            InventoryItem item = inventoryData.GetItemAt(itemIndex);
            if (item.IsEmpty)
            {
                return;
            }
            inventoryUI.CreateDraggedItem(item.UnitSprite);

        }
        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }
        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem item = inventoryData.GetItemAt(itemIndex);
            if (item.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            inventoryUI.UpdateDescription(itemIndex, item);
        }

        public bool ToggleInventory()
        {
            if (inventoryUI.gameObject.activeSelf)
            {
                inventoryUI.Hide();
                return false;
            }
            else
            {
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, item.Value.UnitSprite);
                }
                return true;
            }
        }

        public void ResetInventoryData(List<InventoryItem> newItems)
        {
            inventoryData.ResetInventoryData(newItems);
        }
    }
}