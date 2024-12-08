using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardState
{
    public int MinX => minX;
    public int MinY => minY;
    private Dictionary<Vector3Int, TileBase> tileDictionary;
    private TileBase[,] matrix;
    private int minX, maxX, minY, maxY;

    public BoardState(Dictionary<Vector3Int, TileBase> tileDictionary)
    {
        this.tileDictionary = tileDictionary;
        InitializeMatrix();
    }

    private void InitializeMatrix()
    {
        if (tileDictionary.Count == 0)
        {
            matrix = new TileBase[0, 0];
            return;
        }

        // 找到最小和最大的 x 和 y 值
        minX = int.MaxValue;
        maxX = int.MinValue;
        minY = int.MaxValue;
        maxY = int.MinValue;

        foreach (var kvp in tileDictionary)
        {
            Vector3Int pos = kvp.Key;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // 创建合适大小的二维数组
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;
        matrix = new TileBase[width, height];

        // 填充二维数组
        foreach (var kvp in tileDictionary)
        {
            Vector3Int pos = kvp.Key;
            int x = pos.x - minX;
            int y = pos.y - minY;
            matrix[x, y] = kvp.Value;
        }
    }

    public TileBase[,] GetMatrix()
    {
        return matrix;
    }

    public void SetTile(Vector3Int position, TileBase tile)
    {
        tileDictionary[position] = tile;
        int x = position.x - minX;
        int y = position.y - minY;
        matrix[x, y] = tile;
    }

    public TileBase GetTile(Vector3Int position)
    {
        int x = position.x - minX;
        int y = position.y - minY;
        return matrix[x, y];
    }

    public void ResetTiles(TileBase normalTile)
    {
        // 创建一个临时列表来存储键
        List<Vector3Int> keys = new List<Vector3Int>(tileDictionary.Keys);

        // 遍历临时列表并修改 Dictionary 的内容
        foreach (var key in keys)
        {
            SetTile(key, normalTile);
        }
    }
}