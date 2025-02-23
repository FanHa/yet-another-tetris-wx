using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class TetriCell
    {
        [SerializeField]
        public string Name { get; }

    }
}