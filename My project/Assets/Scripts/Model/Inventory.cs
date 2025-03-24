using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model{

    [CreateAssetMenu]
    [Serializable]
    public class Inventory : ScriptableObject
    {
        [field: SerializeField] private List<InventoryItem> items;

        [field: SerializeField]
        public int Size { get; private set; } = 12;

        public List<InventoryItem> Items
        {
            get => items;
            set => items = value;
        }
        public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

        public void Initialize()
        {
            items = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                items.Add(new InventoryItem());
            }
        }

        private void AddItem(InventoryItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsEmpty)
                {
                    items[i] = item;
                    return;
                }
            }
            // todo 如果找不到空位，应该怎么处理？
        }

        public void AddItems(List<InventoryItem> newItems)
        {
            foreach (var item in newItems)
            {
                AddItem(item);
            }
            InformAboutChange();
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsEmpty)
                {
                    continue;
                }
                returnValue[i] = items[i];
            }
            return returnValue;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {
            InventoryItem temp = items[itemIndex_1];
            items[itemIndex_1] = items[itemIndex_2];
            items[itemIndex_2] = temp;
            InformAboutChange();

        }

        private void InformAboutChange()
        {
            OnInventoryChanged?.Invoke(GetCurrentInventoryState());
        }

        public void ResetInventoryData(List<InventoryItem> newItems)
        {
            Initialize();
            AddItems(newItems);
        }
    }

}