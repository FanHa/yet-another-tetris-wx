using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Attribute : TetriCell
    {
        public abstract void ApplyAttributes(Unit unit);

    }
}