using System;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class Serializable2DArray
    {
        [SerializeField]
        private int rows;
        [SerializeField]
        private int cols;
        [SerializeField]
        private int[] array;

        public Serializable2DArray(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            array = new int[rows * cols];
        }

        public int this[int row, int col]
        {
            get { return array[row * cols + col]; }
            set { array[row * cols + col] = value; }
        }

        public int Rows => rows;
        public int Cols => cols;

        public int GetLength(int v)
        {
            if (v == 0)
            {
                return rows;
            }
            else if (v == 1)
            {
                return cols;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}