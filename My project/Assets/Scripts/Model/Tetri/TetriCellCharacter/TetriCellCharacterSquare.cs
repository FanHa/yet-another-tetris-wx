using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCharacterSquare : TetriCellCharacter
    {


        public override object Clone()
        {
            return new TetriCellCharacterSquare();
        }
    }
}