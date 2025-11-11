using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model{

    [CreateAssetMenu]
    [Serializable]
    public class UnitInventoryModel : ScriptableObject
    {
        [field: SerializeField] private List<CharacterPlacement> items = new List<CharacterPlacement>();

        public List<CharacterPlacement> Items
        {
            get => items;
            set => items = value;
        }
        public event Action<List<CharacterPlacement>> OnDataChanged;

        public void AddItems(List<CharacterPlacement> newItems)
        {
            items.AddRange(newItems);
            OnDataChanged?.Invoke(items);
        }

        public CharacterPlacement GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void ResetInventoryData(List<CharacterPlacement> newItems)
        {
            items.Clear();
            AddItems(newItems);
        }
    }

}