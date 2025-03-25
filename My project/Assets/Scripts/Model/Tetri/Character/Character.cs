using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : TetriCell
    {
        public abstract void ApplyAttributes(Unit unitComponent);
    }
}