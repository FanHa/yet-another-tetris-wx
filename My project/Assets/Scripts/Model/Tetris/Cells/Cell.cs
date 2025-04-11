using System;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Cell
    {

        public abstract string Description();
        public abstract string Name();

        public abstract void Apply(Units.Unit unit); // Apply方法返回一个字符串，表示应用效果的描述

    }
}