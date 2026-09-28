using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class EmptySlot : Cell
    {
        public EmptySlot()
        {
            // 可以在这里添加初始化逻辑
        }

        public override void Apply(Unit unit)
        {
            
        }

        public override string Description()
        {
            return "Empty slot";
        }

        public override string Name()
        {
            return "Empty slot";
        }
    }
}