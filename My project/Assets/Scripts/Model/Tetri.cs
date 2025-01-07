using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model{
    [Serializable]
    public class Tetri : ICloneable
    {
        [SerializeField]
        public Serializable2DArray shape;

        public Serializable2DArray Shape => shape;

        public Tetri()
        {
            shape = new Serializable2DArray(4, 4);
        }

        public object Clone()
        {
            return new Tetri
            {
                shape = (Serializable2DArray)this.shape.Clone()
            };
        }
    }
}