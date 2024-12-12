using System.Collections.Generic;
using UnityEngine;

public class Tetri
{
    public int[,] Shape { get; private set; }
    public List<Vector3Int> RelativePositions { get; private set; }

    public Tetri(int[,] shape)
    {
        Shape = shape;
        RelativePositions = CalculateRelativePositions();
    }

    private List<Vector3Int> CalculateRelativePositions()
    {
        List<Vector3Int> relativePositions = new List<Vector3Int>();
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (Shape[row, col] == 1)
                {
                    relativePositions.Add(new Vector3Int(col, row, 0));
                }
            }
        }
        return relativePositions;
    }

    public int[,] GetRotatedShape(int rotationState)
    {
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);
        int[,] rotatedShape = Shape;

        for (int i = 0; i < rotationState; i++)
        {
            rotatedShape = RotateClockwise(rotatedShape);
        }

        return rotatedShape;
    }

    private int[,] RotateClockwise(int[,] shape)
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        int[,] rotatedShape = new int[cols, rows];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                rotatedShape[col, rows - 1 - row] = shape[row, col];
            }
        }

        return rotatedShape;
    }
}