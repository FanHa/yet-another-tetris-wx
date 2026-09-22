using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model{

    [CreateAssetMenu]
    [Serializable]
    public class UnitInventoryModel : ScriptableObject
    {
        [field: SerializeField] private List<CharacterPlacement> items = new List<CharacterPlacement>();

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