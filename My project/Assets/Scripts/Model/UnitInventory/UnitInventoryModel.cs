using System;
using System.Collections.Generic;

namespace Model.UnitInventory
{
    [Serializable]
    public class UnitInventoryModel
    {
        private readonly List<CharacterPlacement> items = new();

        public IReadOnlyList<CharacterPlacement> Items => items;
        public event Action<IReadOnlyList<CharacterPlacement>> OnDataChanged;

        public void AddItems(IReadOnlyList<CharacterPlacement> newItems)
        {
            items.AddRange(newItems);
            OnDataChanged?.Invoke(items);
        }

        public CharacterPlacement GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void ResetInventoryData(IReadOnlyList<CharacterPlacement> newItems)
        {
            items.Clear();
            AddItems(newItems);
        }
    }
}