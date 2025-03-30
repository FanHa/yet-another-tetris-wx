using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Square : Character
    {
        public override string Description()
        {
            return "base moderation character"; // 返回一个字符串
        }
        public override string CharacterDescription()
        {
            return Description();
        }
    }
}