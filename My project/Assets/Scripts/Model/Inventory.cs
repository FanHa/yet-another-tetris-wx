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

        public IReadOnlyList<InventoryItem> Items => items;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

        public void Initialize()
        {
            items = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                items.Add(new InventoryItem());
            }
        }

        public void AddCombatUnit(CombatUnit unit)
        {
            for (int i=0; i<items.Count; i++)
            {
                if (items[i].IsEmpty)
                {
                    var newItem = new InventoryItem();
                    newItem.Unit = unit;
                    items[i] = newItem;
                    return;
                }
            }
            // todo 如果找不到空位，应该怎么处理？
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
    }

}