using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model{

    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> items;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

        public void Initialize()
        {
            items = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                items.Add(InventoryItem.GetEmptyItem());
            }
        }

        public void AddItem(UnitSO unit, int quantity)
        {
            for (int i=0; i<items.Count; i++)
            {
                if (items[i].IsEmpty)
                {
                    items[i] = new InventoryItem
                    {
                        unit = unit,
                        quantity = quantity
                    };
                    return;
                }
            }
            InformAboutChange();
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.unit, item.quantity);
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

[Serializable]
public struct InventoryItem
{
    public int quantity;
    public UnitSO unit;

    public bool IsEmpty => unit == null;

    public InventoryItem ChangeQuantity(int newValue)
    {
        return new InventoryItem
        {
            quantity = newValue,
            unit = unit
        };
    }

    public static InventoryItem GetEmptyItem()
    {
        return new InventoryItem
        {
            quantity = 0,
            unit = null
        };
    }
}
}