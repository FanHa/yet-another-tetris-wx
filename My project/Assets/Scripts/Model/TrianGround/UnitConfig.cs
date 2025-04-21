using System.Collections.Generic;
using UnityEngine;


namespace Model.TrainGround
{
    [System.Serializable]
    public class UnitConfig
    {
        public CharacterTypeReference CharacterCell; // 单位的 CharacterCell
        public List<CellTypeReference> TetriCells;   // 单位的 TetriCells 列表

        public InventoryItem ToInventoryItem()
        {
            // 实例化 CharacterCell
            var characterCell = CharacterCell?.CreateInstance();

            // 实例化 TetriCells
            var tetriCells = new List<Model.Tetri.Cell>();
            if (TetriCells != null)
            {
                foreach (var cellType in TetriCells)
                {
                    var cellInstance = cellType?.CreateInstance();
                    if (cellInstance != null)
                    {
                        tetriCells.Add(cellInstance);
                    }
                }
            }

            // 创建并返回 InventoryItem
            return new InventoryItem(characterCell, tetriCells);
        }
    }
}