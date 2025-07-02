using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;


namespace Model.TrainGround
{
    [System.Serializable]
    public class UnitConfig
    {
        public CharacterTypeId characterId; // 单位的 CharacterCell
        public List<CellTypeId> tetriCellIds;   // 单位的 TetriCells 列表

        public InventoryItem ToInventoryItem(TetriCellFactory factory)
        {
            // 实例化 CharacterCell
            var characterCell = factory.CreateCharacterCell(characterId);

            // 实例化 TetriCells
            var tetriCells = new List<Model.Tetri.Cell>();
            if (tetriCellIds != null)
            {
                foreach (var cellId in tetriCellIds)
                {
                    var cellInstance = factory.CreateCell(cellId);
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