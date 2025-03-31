using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;
using System.Linq;

namespace Controller {
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private UI.Inventories.Inventory inventoryUI;
        [SerializeField] private Model.Inventory inventoryData;

        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;

        private static bool isInitialized = false;
        
        private void Start()
        {
            if (!isInitialized)
            {
                PrepareInventoryData();
                isInitialized = true;
            }
        }


        public Model.InventoryItem GenerateInventoryItemFromTetriCells(List<Cell> tetriCells)
        {
            Character characterCell = tetriCells.OfType<Character>().FirstOrDefault();
            int spawnInterval = 0; // 根据需要设置
            return new Model.InventoryItem(characterCell, spawnInterval, tetriCells);
        }

   
        private void PrepareInventoryData()
        {
            inventoryData.OnInventoryChanged += UpdateInventoryUI;
        }
        private void UpdateInventoryUI(List<Model.InventoryItem> inventoryState)
        {
            inventoryUI.UpdateData(inventoryState);
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
                inventoryUI.UpdateData(inventoryData.Items);
                return true;
            }
        }

        public void Hide()
        {
            inventoryUI.Hide();
        }

        public void ResetInventoryData(List<InventoryItem> newItems)
        {
            inventoryData.ResetInventoryData(newItems);
        }
    }
}