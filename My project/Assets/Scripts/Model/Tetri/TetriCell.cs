using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class TetriCell :ScriptableObject, ICloneable
    {
        public abstract object Clone();
    }
}