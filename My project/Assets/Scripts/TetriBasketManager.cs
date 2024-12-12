using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TetriBasketManager : MonoBehaviour
{
    [SerializeField] private TileBase basketTile;
    private Dictionary<int, TetriBasket> baskets;

    private Tilemap tilemap;

    private int nextBasketId;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        baskets = new Dictionary<int, TetriBasket>();
        nextBasketId = 0;
        // Initialize();
    }

    public void Initialize()
    {
        int rows = 3;
        int cols = 4;
        int basketSize = 4;
        int spacing = 1; // 每个篮子之间的间隔

        int totalHeight = rows * (basketSize + spacing) - spacing;
        int offsetY = -totalHeight / 2;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // 计算篮子的基准位置，间隔一个格子，以 x 轴为中轴对称排列
                Vector3Int basePosition = new Vector3Int(col * (basketSize + spacing), offsetY + row * (basketSize + spacing), 0);
                TetriBasket basket = new TetriBasket(basePosition);
                int basketId = nextBasketId++;
                baskets[basketId] = basket;

                // 绘制篮子
                for (int x = 0; x < basketSize; x++)
                {
                    for (int y = 0; y < basketSize; y++)
                    {
                        Vector3Int tilePosition = basePosition + new Vector3Int(x, y, 0);
                        tilemap.SetTile(tilePosition, basketTile);
                    }
                }
            }
        }
    }

    public TetriBasket GetBasketById(int id)
    {
        baskets.TryGetValue(id, out TetriBasket basket);
        return basket;
    }

    public TetriBasket GetFreeBasket()
    {
        foreach (var basket in baskets.Values)
        {
            if (!basket.IsOccupied)
            {
                return basket;
            }
        }
        return null;
    }

}

 

public class TetriBasket
{
    public Vector3Int BasePosition { get; private set; }
    public bool IsOccupied { get; set; }

    public TetriBasket(Vector3Int basePosition)
    {
        BasePosition = basePosition;
    }

    public List<Vector3Int> GetTiles()
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                tiles.Add(new Vector3Int(BasePosition.x + x, BasePosition.y + y, BasePosition.z));
            }
        }
        return tiles;
    }
}