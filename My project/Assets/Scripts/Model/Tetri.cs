using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model{
    [Serializable]
    public class Tetri : ICloneable
    {
        [SerializeField]
        public Serializable2DArray<int> shape;

        public Serializable2DArray<int> Shape => shape;

        public Tetri()
        {
            shape = new Serializable2DArray<int>(4, 4);
        }

        public object Clone()
        {
            return new Tetri
            {
                shape = (Serializable2DArray<int>)this.shape.Clone()
            };
        }
    }
}