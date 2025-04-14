using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Tetri
    {
        [SerializeField] private Serializable2DArray<Cell> shape;

        public Serializable2DArray<Cell> Shape => shape;
        [SerializeField] private bool isDisposable;
        public bool IsDisposable => isDisposable;

        public Tetri(bool isDisposable = false)
        {
            this.isDisposable = isDisposable;
            shape = new Serializable2DArray<Cell>(4, 4);
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    shape[i, j] = new Empty();
                }
            }
        }
        public void SetCell(int row, int column, Cell cell)
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

        internal List<Vector2Int> GetOccupiedPositions()
        {
            var occupiedPositions = new List<Vector2Int>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (!(shape[i, j] is Empty))
                    {
                        occupiedPositions.Add(new Vector2Int(i, j));
                    }
                }
            }
            return occupiedPositions;
        }
    }
}