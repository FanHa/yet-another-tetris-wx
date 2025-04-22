using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        public Triangle()
        {
            AttackPowerValue = 15f;
            MaxCoreValue = 60f;
        }
        
        public override string Name()
        {
            return "小三";
        }
    }
}