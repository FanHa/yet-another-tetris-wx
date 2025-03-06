using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellEmpty : TetriCell
    {
        public TetriCellEmpty()
        {
            // 可以在这里添加初始化逻辑
        }

        public override string Description()
        {
            return "Empty"; // 返回一个字符串
        }
    }
}