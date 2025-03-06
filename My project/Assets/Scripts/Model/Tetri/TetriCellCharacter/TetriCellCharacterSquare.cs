using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCharacterSquare : TetriCellCharacter
    {
        public override string Description()
        {
            return "Square"; // 返回一个字符串
        }
    }
}