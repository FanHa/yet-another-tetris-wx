using System;
using System.Collections.Generic;

namespace Model.UnitInventory
{
    [Serializable]
    public class UnitInventoryModel
    {
        private readonly List<UnitSpawnPlacement> items = new();

        public IReadOnlyList<UnitSpawnPlacement> Items => items;
        public event Action<IReadOnlyList<UnitSpawnPlacement>> OnDataChanged;

        public void AddItems(IReadOnlyList<UnitSpawnPlacement> newItems)
        {
            items.AddRange(newItems);
            OnDataChanged?.Invoke(items);
        }

        public UnitSpawnPlacement GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void ResetInventoryData(IReadOnlyList<UnitSpawnPlacement> newItems)
        {
            items.Clear();
            AddItems(newItems);
        }
    }
}