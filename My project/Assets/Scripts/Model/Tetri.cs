using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model{
    [Serializable]
    public class Tetri
    {
        [SerializeField]
        public Serializable2DArray shape;

        public Serializable2DArray Shape => shape;

        public Tetri()
        {
            shape = new Serializable2DArray(4, 4);
        }

        public Tetri(int rows, int cols)
        {
            shape = new Serializable2DArray(rows, cols);
        }
    }
}