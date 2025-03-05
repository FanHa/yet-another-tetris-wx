using System;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class TetriCell
    {
        [SerializeField]
        public string Name { get; }

        public string Description()
        {
            return "Test";
        }

    }
}