using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellAttribute : TetriCell
    {
        [SerializeField]
        public string attribute; // 属性

        public TetriCellAttribute(string attribute)
        {
            this.attribute = attribute;
        }

        public override object Clone()
        {
            return new TetriCellAttribute(attribute);
        }
    }
}