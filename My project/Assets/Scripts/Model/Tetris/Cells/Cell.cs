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

    }
}