using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;
using System.Linq;

namespace Controller {
    public class UnitInventoryController : MonoBehaviour
    {
        private UI.Inventories.UnitInventoryView inventoryView;
        [SerializeField] private Model.UnitInventoryModel inventoryData;
        [SerializeField] private Units.UnitFactory unitFactory;

        private void Awake()
        {
            inventoryView = GetComponent<UI.Inventories.UnitInventoryView>();
            if (inventoryView == null)
            {
                Debug.LogError("UnitInventoryController requires a UnitInventoryView component.");
            }
        }
        private void Start()
        {
            inventoryData.OnDataChanged += HandleDataChange;
        }


        private Model.InventoryItem GenerateInventoryItemFromTetriCells(List<Cell> tetriCells)
        {
            Character characterCell = tetriCells.OfType<Character>().FirstOrDefault();
            List<Cell> remainingCells = tetriCells.Where(cell => cell != characterCell).ToList();

            return new Model.InventoryItem(characterCell, remainingCells);
        }

        public void ResetInventoryDataFromCellGroups(List<List<Cell>> cellGroups)
        {
            var items = new List<InventoryItem>();
            foreach (var group in cellGroups)
            {
                var item = GenerateInventoryItemFromTetriCells(group);
                items.Add(item);
            }
            inventoryData.ResetInventoryData(items);

        }


        private void HandleDataChange(List<Model.InventoryItem> inventoryState)
        {
            var unitList = new List<Units.Unit>();

            foreach (Model.InventoryItem item in inventoryState)
            {
                Units.Unit unit = unitFactory.CreateUnit(item);
                if (unit != null)
                {
                    unitList.Add(unit);
                }
            }
            inventoryView.UpdateData(unitList);
        }

        // public bool ToggleInventory()
        // {
        //     if (inventoryView.gameObject.activeSelf)
        //     {
        //         inventoryView.Hide();
        //         return false;
        //     }
        //     else
        //     {
        //         inventoryView.Show();
        //         inventoryView.UpdateData(inventoryData.Items);
        //         return true;
        //     }
        // }

        public void Hide()
        {
            inventoryView.Hide();
        }

        //  void ResetInventoryData(List<InventoryItem> newItems)
        // {
        //     inventoryData.ResetInventoryData(newItems);
        // }
    }
}