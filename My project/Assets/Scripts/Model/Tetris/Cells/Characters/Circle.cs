using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character
    {
        public Circle()
        {
            // 设置基类的字段
            AttackPowerValue = 7f;  // 设置攻击力
            MaxCoreValue = 150f;   // 设置最大生命值
        }

        public override string Name()
        {
            return "小圆";
        }


    }
}