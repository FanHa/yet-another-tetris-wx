using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellBasic : TetriCell
    {
        public TetriCellBasic()
        {
            type = CellType.Basic;
        }

        public override object Clone()
        {
            return new TetriCellBasic();
        }
    }
}