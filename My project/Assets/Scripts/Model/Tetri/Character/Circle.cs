using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character
    {
        public override string Description()
        {
            return "Circle"; // 返回一个字符串
        }
    }
}