using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;
using System.Linq;

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


        public InventoryItem GenerateInventoryItemFromTetriCells(List<Cell> tetriCells)
        {
            Character characterCell = tetriCells.OfType<Character>().FirstOrDefault();

            // 根据唯一的 TetriCellCharacter 生成 InventoryItem
            Type characterType = characterCell.GetType();
            string unitName = $"Generated Unit ({characterType.Name})";
            Sprite unitSprite = cellTypeResourceMapping.GetSprite(characterType); // 根据需要设置
            GameObject prefab = cellTypeResourceMapping.GetPrefab(characterType); // 根据需要设置
            string description = $"Generated from {characterType.Name} cell";
            int spawnInterval = 0; // 根据需要设置

            return new InventoryItem(unitName, unitSprite, prefab, description, spawnInterval, tetriCells);
        }



        private void PrepareUI()
        {
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
            inventoryUI.UpdateData(inventoryState);
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
                inventoryUI.UpdateData(inventoryData.GetCurrentInventoryState());
                return true;
            }
        }

        public void ResetInventoryData(List<InventoryItem> newItems)
        {
            inventoryData.ResetInventoryData(newItems);
        }
    }
}