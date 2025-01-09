

using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Controller.BattlefieldController{
    public class InventoryController : MonoBehaviour
    {
        public List<Model.CombatUnit> initialCombatUnits = new List<Model.CombatUnit>();
        [SerializeField] private UI.Inventory inventoryUI;
        [SerializeField] private Model.Inventory inventoryData;
        
        private void Start()
        {
            PrepareUI();
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
            foreach (var unit in initialCombatUnits)
            {
                inventoryData.AddCombatUnit(unit);
            }
        }
        private void UpdateInventoryUI(Dictionary<int, Model.InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.Unit.UnitSprite);
            }
        }
        private void HandleItemActionRequest(int itemIndex)
        {
            throw new NotImplementedException();
        }
        private void HandleStartDragging(int itemIndex)
        {
            Model.InventoryItem item = inventoryData.GetItemAt(itemIndex);
            if (item.IsEmpty)
            {
                return;
            }
            inventoryUI.CreateDraggedItem(item.Unit.UnitSprite);
        }
        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }
        private void HandleDescriptionRequest(int itemIndex)
        {
            Model.InventoryItem item = inventoryData.GetItemAt(itemIndex);
            if (item.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            CombatUnit unit = item.Unit;
            inventoryUI.UpdateDescription(itemIndex, unit.UnitSprite, unit.UnitName, unit.Description);
        }
        public void Update()
        {
            
        }
        public void InventoryAction()
        {
            if (inventoryUI.gameObject.activeSelf)
            {
                inventoryUI.Hide();
            }
            else
            {
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, item.Value.Unit.UnitSprite);
                }
            }
        }
    }
}