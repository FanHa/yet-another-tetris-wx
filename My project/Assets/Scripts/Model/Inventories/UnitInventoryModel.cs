using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model{

    [CreateAssetMenu]
    [Serializable]
    public class UnitInventoryModel : ScriptableObject
    {
        [field: SerializeField] private List<CharacterInfluenceGroup> items = new List<CharacterInfluenceGroup>();

        public List<CharacterInfluenceGroup> Items
        {
            get => items;
            set => items = value;
        }
        public event Action<List<CharacterInfluenceGroup>> OnDataChanged;

        public void AddItems(List<CharacterInfluenceGroup> newItems)
        {
            items.AddRange(newItems);
            OnDataChanged?.Invoke(items);
        }

        public CharacterInfluenceGroup GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public void ResetInventoryData(List<CharacterInfluenceGroup> newItems)
        {
            items.Clear();
            AddItems(newItems);
        }
    }

}