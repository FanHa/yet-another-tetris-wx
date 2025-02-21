using System;
using UnityEngine;
using Units;

namespace Model
{
    [Serializable]
    public class InventoryItem
    {
        [field: SerializeField] public string UnitName { get; set; }
        [field: SerializeField] public Sprite UnitSprite { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField, TextArea] public string Description { get; set; }
        [SerializeField] public int spawnInterval;

        public bool IsEmpty => string.IsNullOrEmpty(UnitName);

        // 构造函数
        public InventoryItem(string unitName = null, Sprite unitSprite = null, GameObject prefab = null, string description = null, int spawnInterval = 0)
        {
            UnitName = unitName;
            UnitSprite = unitSprite;
            Prefab = prefab;
            Description = description;
            this.spawnInterval = spawnInterval;
        }
    }
    
}