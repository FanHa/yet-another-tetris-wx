using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellBasic : TetriCell
    {

        public override string Description()
        {
            return "Basic"; // 返回一个字符串
        }
    }
}