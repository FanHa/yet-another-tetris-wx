using System;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class TetriCell : ICloneable
    {
        public enum CellType
        {
            Empty,
            Square,
            Circle,
            Triangle,
            Fire,
            Water,
            Ice,
            // 其他类型...
        }

        [SerializeField]
        public CellType type = CellType.Empty; // 类别

        // 你可以添加更多的属性和特技
        [SerializeField]
        public string attribute; // 属性

        [SerializeField]
        public string specialSkill; // 特技


        public object Clone()
        {
            return new TetriCell
            {
                type = this.type,
                attribute = this.attribute,
                specialSkill = this.specialSkill
                // 复制其他属性
            };
        }
    }
}