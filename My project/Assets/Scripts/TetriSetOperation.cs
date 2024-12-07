using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOperation
{
    public Vector3Int Position { get; set; }
    public TileBase OriginalTile { get; set; }

    public TileOperation(Vector3Int position, TileBase originalTile)
    {
        Position = position;
        OriginalTile = originalTile;
    }
}
