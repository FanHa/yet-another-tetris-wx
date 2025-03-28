using System;
using UnityEngine;
using Units;
using System.Collections.Generic;
using Model.Rewards;

namespace Model
{
    [Serializable]
    public class InventoryItem
    {
        [SerializeField] public Model.Tetri.Character CharacterCell { get; set; }
        [SerializeField] public int spawnInterval;
        [SerializeField] private List<Tetri.Cell> tetriCells = new List<Tetri.Cell>();
        // 公共属性确保 tetriCells 永远不为 null
        public List<Tetri.Cell> TetriCells
        {
            get
            {
                if (tetriCells == null)
                {
                    tetriCells = new List<Tetri.Cell>();
                }
                return tetriCells;
            }
            set
            {
                tetriCells = value ?? new List<Tetri.Cell>();
            }
        }

        // 构造函数
        public InventoryItem(
            Model.Tetri.Character characterCell = null,
            int spawnInterval = 0,
            List<Tetri.Cell> tetriCells = null)
        {
            
            this.CharacterCell = characterCell;
            this.spawnInterval = spawnInterval;
            this.tetriCells = tetriCells ?? new List<Tetri.Cell>(); // 确保 tetriCells 不为 null
        }
    }
    
}