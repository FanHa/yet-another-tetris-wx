using System;
using UnityEngine;
using Units;
using System.Collections.Generic;

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
        [SerializeField] private List<Tetri.TetriCell> tetriCells = new List<Tetri.TetriCell>();
        // 公共属性确保 tetriCells 永远不为 null
        public List<Tetri.TetriCell> TetriCells
        {
            get
            {
                if (tetriCells == null)
                {
                    tetriCells = new List<Tetri.TetriCell>();
                }
                return tetriCells;
            }
            set
            {
                tetriCells = value ?? new List<Tetri.TetriCell>();
            }
        }
        public bool IsEmpty => string.IsNullOrEmpty(UnitName);

        // 构造函数
        public InventoryItem(
            string unitName = null, 
            Sprite unitSprite = null,
            GameObject prefab = null, 
            string description = null, 
            int spawnInterval = 0,
            List<Tetri.TetriCell> tetriCells = null)
        {
            UnitName = unitName;
            UnitSprite = unitSprite;
            Prefab = prefab;
            Description = description;
            this.spawnInterval = spawnInterval;
            this.tetriCells = tetriCells ?? new List<Tetri.TetriCell>(); // 确保 tetriCells 不为 null
        }
    }
    
}