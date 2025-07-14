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
        public event Action<Unit> OnUnitClicked;
        [SerializeField] private Transform contentPanel;
        List<InventoryItem> items = new List<InventoryItem>();

        public void UpdateData(List<Unit> unitList)
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
                unit.OnClicked += HandleUnitClicked;
            }
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        private Vector3 CalculateGridPosition(int index)
        {
            int perRow = 3;
            int rows = 2;
            float width = 4.5f;
            float height = 3.5f;
            float spacing = 0.5f;
            float cellWidth = width + spacing;
            float cellHeight = height + spacing;

            float totalWidth = perRow * cellWidth - spacing;
            float totalHeight = rows * cellHeight - spacing;

            float offsetX = -totalWidth / 2 + cellWidth / 2;
            float offsetY = totalHeight / 2 - cellHeight / 2;

            int row = index / perRow;
            int col = index % perRow;

            return new Vector3(col * cellWidth, -row * cellHeight, 0) + new Vector3(offsetX, offsetY, 0);
        }

        // private void HandleItemSelection(InventoryItem item)
        // {
        //     int index = items.IndexOf(item);
        //     if (index == -1)
        //     {
        //         Debug.LogError("Item not found");
        //         return;
        //     }
        //     Model.UnitInventoryItem selectedItem = items[index].Data;
        //     itemDescription.SetDescription(selectedItem); // Directly set the description
        //     DeselectAllItems();
        //     items[index].Select();
        //     OnDescriptionRequested?.Invoke(index);

        // }

        private void DeselectAllItems()
        {
            foreach (var item in items)
            {
                item.Deselect();
            }
        }

        
    }
}
