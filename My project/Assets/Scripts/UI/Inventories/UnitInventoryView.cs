using System;
using System.Collections;
using System.Collections.Generic;
using Model.Tetri;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventories
{

    public class UnitInventoryView : MonoBehaviour
    {
        public event Action<int> OnDescriptionRequested;
        // [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private Transform contentPanel;
        [SerializeField] private Description.Description itemDescription;
        // [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;
        List<InventoryItem> items = new List<InventoryItem>();


        private void Awake()
        {
            // Hide();
        }

        // public void UpdateData(List<Model.InventoryItem> inventoryState)
        // {
        //     ResetAllItems();

        //     // Update or create UI elements
        //     foreach (Model.InventoryItem item in inventoryState)
        //     {
        //         UI.Inventories.InventoryItem itemUI = Instantiate(itemPrefab, contentPanel);
        //         itemUI.SetData(item, tetriCellTypeResourceMapping.GetSprite(item.CharacterCell));
        //         items.Add(itemUI);
        //         itemUI.OnItemClicked += HandleItemSelection;
        //     }
        // }

        internal void UpdateData(List<Unit> unitList)
        {
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < unitList.Count; i++)
            {
                var unit = unitList[i];
                unit.transform.SetParent(contentPanel, false);
                unit.transform.localPosition = CalculateGridPosition(i);
            }
        }

        private Vector3 CalculateGridPosition(int index)
        {
            int perRow = 3;
            float size = 5f;
            float spacing = 0.5f;
            float cellSize = size + spacing;

            int row = index / perRow;
            int col = index % perRow;

            return new Vector3(col * cellSize, -row * cellSize, 0);
        }

        private void HandleItemSelection(InventoryItem item)
        {
            int index = items.IndexOf(item);
            if (index == -1)
            {
                Debug.LogError("Item not found");
                return;
            }
            Model.InventoryItem selectedItem = items[index].Data;
            itemDescription.SetDescription(selectedItem); // Directly set the description
            DeselectAllItems();
            items[index].Select();
            OnDescriptionRequested?.Invoke(index);

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
        }

        private void ResetAllItems()
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
        }

        
    }
}
