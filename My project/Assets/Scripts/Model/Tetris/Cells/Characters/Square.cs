using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Square : Character
    {
        public override string Description()
        {
            return "Base Character"; // 返回一个字符串
        }

        public override void ApplyFeatures(Units.Unit unitComponent)
        {
            // Do nothing
        }

        public override string CharacterDescription()
        {
            return Description();
        }
    }
}