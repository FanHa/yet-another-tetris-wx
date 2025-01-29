using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCharacter : TetriCell
    {
        public override object Clone()
        {
            return new TetriCellCharacter();
        }
    }
}