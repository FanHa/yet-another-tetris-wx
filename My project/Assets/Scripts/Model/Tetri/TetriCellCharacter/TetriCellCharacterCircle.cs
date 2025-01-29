using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCharacterCircle : TetriCellCharacter
    {
        public override object Clone()
        {
            return new TetriCellCharacterCircle();
        }
    }
}