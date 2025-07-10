using System;
using UnityEngine;
using Units;
using System.Collections.Generic;
using Model.Rewards;

namespace Model
{
    [Serializable]
    public class UnitInventoryItem
    {
        [SerializeField] public Model.Tetri.Character CharacterCell { get; set; }
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
        public UnitInventoryItem(
            Model.Tetri.Character characterCell,
            List<Tetri.Cell> tetriCells)
        {
            
            this.CharacterCell = characterCell;
            this.tetriCells = tetriCells;
        }
    }
    
}