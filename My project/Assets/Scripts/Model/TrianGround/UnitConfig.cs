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
    }
}