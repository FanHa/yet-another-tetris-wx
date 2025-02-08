using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Tetri
    {
        [SerializeField]
        private Serializable2DArray<TetriCell> shape;

        public Serializable2DArray<TetriCell> Shape => shape;

        public Tetri()
        {
            shape = new Serializable2DArray<TetriCell>(4, 4);
            InitializeShape();
        }

        private void InitializeShape()
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    shape[i, j] = new TetriCellEmpty();
                }
            }
        }
        public void SetCell(int row, int column, TetriCell cell)
        {
            if (row >= 0 && row < shape.GetLength(0) && column >= 0 && column < shape.GetLength(1))
            {
                shape[row, column] = cell;
            }
            else
            {
                Debug.LogWarning("Invalid row or column index.");
            }
        }
    }
}