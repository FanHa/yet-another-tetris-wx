using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Empty : Cell
    {
        public Empty()
        {
            // 可以在这里添加初始化逻辑
        }

        public override void Apply(Unit unit)
        {
            
        }

        public override string Description()
        {
            return "Empty"; // 返回一个字符串
        }

        public override string Name()
        {
            return "Empty";
        }
    }
}