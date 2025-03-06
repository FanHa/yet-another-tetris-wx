using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCharacterTriangle : TetriCellCharacter
    {
        public override string Description()
        {
            return "Triangle"; // 返回一个字符串
        }
    }
}