using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model{

    [CreateAssetMenu]
    [Serializable]
    public class Inventory : ScriptableObject
    {
        [field: SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

        public List<InventoryItem> Items
        {
            get => items;
            set => items = value;
        }
        public event Action<List<InventoryItem>> OnInventoryChanged;

        public void AddItems(List<InventoryItem> newItems)
        {
            items.AddRange(newItems);
            OnInventoryChanged?.Invoke(items);
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void ResetInventoryData(List<InventoryItem> newItems)
        {
            items.Clear();
            AddItems(newItems);
        }
    }

}