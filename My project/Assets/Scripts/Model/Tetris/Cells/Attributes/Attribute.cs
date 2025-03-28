using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Attribute : Cell, IBaseAttribute
    {
        public abstract void ApplyAttributes(Unit unit);

    }
}