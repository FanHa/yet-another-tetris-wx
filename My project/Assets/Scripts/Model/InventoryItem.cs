using System;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class InventoryItem
    {
        [field: SerializeField] public CombatUnit Unit { get; set; }
        [SerializeField] public int spawnInterval;

        public bool IsEmpty => Unit == null;

    }
    
}