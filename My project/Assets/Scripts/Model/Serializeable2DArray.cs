using System;
using System.Runtime.Serialization;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class Serializable2DArray<T>
    {
        [SerializeField] private int rows;
        [SerializeField] private int cols;
        [SerializeField] private T[] array; // 改成SerializeReference

        public Serializable2DArray(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            array = new T[rows * cols];
        }

        public T this[int row, int col]
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