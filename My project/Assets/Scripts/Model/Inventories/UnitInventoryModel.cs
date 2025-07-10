using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model{

    [CreateAssetMenu]
    [Serializable]
    public class UnitInventoryModel : ScriptableObject
    {
        [field: SerializeField] private List<UnitInventoryItem> items = new List<UnitInventoryItem>();

        public List<UnitInventoryItem> Items
        {
            get => items;
            set => items = value;
        }
        public event Action<List<UnitInventoryItem>> OnDataChanged;

        public void AddItems(List<UnitInventoryItem> newItems)
        {
            items.AddRange(newItems);
            OnDataChanged?.Invoke(items);
        }

        public UnitInventoryItem GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void ResetInventoryData(List<UnitInventoryItem> newItems)
        {
            items.Clear();
            AddItems(newItems);
        }
    }

}