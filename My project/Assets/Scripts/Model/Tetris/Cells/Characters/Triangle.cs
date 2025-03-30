using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        public override string Description()
        {
            // 动态生成描述字符串，反映当前的提升和降低百分比
            return $"attack power boost, health reduction";
        }

        public override string CharacterDescription()
        {
            return Description();
        }
    }
}