using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Square : Character
    {

        public override string Name()
        {
            return "小方";
        }
    }
}