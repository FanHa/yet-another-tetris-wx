using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetri
{
    public Vector3Int Base { get; set; }
    public List<Vector3Int> RelativePositions { get; set; }

    public Tetri(Vector3Int center)
    {
        Base = center;
        RelativePositions = new List<Vector3Int>();
    }

    public void AddTile(Vector3Int position)
    {
        RelativePositions.Add(position - Base);
    }

    public override string ToString()
    {
        string result = $"Base: {Base}\nRelative Positions:";
        foreach (var pos in RelativePositions)
        {
            result += $"\n{pos}";
        }
        return result;
    }
}