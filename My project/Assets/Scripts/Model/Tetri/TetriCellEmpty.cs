using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellEmpty : TetriCell
    {
        public override object Clone()
        {
            return new TetriCellEmpty();
        }
    }
}