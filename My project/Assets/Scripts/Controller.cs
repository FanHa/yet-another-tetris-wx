using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    [SerializeField] private Tilemap operateBoard;
    [SerializeField] private Tilemap stuffBoard;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private TileBase normalTile;
    [SerializeField] private TileBase setedTile;
    [SerializeField] private TileBase forbidTile;
    private Vector3Int previousMousePos = new Vector3Int();
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    private Tetri currentTetri;
    private List<Vector3Int> highlightedTiles = new List<Vector3Int>();
    void Start()
    {
        InitializeOriginalTiles();
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = operateBoard.WorldToCell(mouseWorldPos);

        if (mouseCellPos != previousMousePos)
        {
            // 恢复之前高亮的瓦片及其相邻瓦片
            RestoreHighliteedTiles();

            // 检查鼠标是否在 operateBoard 上
            if (IsMouseOnOperateBoard(mouseCellPos))
            {
                // 高亮当前瓦片及其相邻瓦片
                // 根据选中的 tetri 的形状来高亮瓦片
                HighlightTile(mouseCellPos);
            }
            previousMousePos = mouseCellPos;
        }

        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == operateBoard.gameObject)
                {
                    SetTileAndNeighbors(mouseCellPos, setedTile);
                }
                else if (hit.collider.gameObject == stuffBoard.gameObject)
                {
                    Tetri tetri = selectTetri(mouseCellPos);
                    currentTetri = tetri;
                }
            }

        }
    }

    private bool IsMouseOnOperateBoard(Vector3Int mouseCellPos)
    {
        BoundsInt bounds = operateBoard.cellBounds;
        return bounds.Contains(mouseCellPos) && operateBoard.HasTile(mouseCellPos);
    }


    private void InitializeOriginalTiles()
    {
        // 初始化 originalTiles 字典
        BoundsInt bounds = operateBoard.cellBounds;
        TileBase[] allTiles = operateBoard.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    originalTiles[position] = tile;
                }
            }
        }
    }

    private void RestoreHighliteedTiles()
    {
        foreach (var tilePosition in highlightedTiles)
        {
            RestoreTile(tilePosition);
        }
        highlightedTiles.Clear();
    }

    private void RestoreTile(Vector3Int position)
    {
        if (operateBoard.HasTile(position))
        {
            if (originalTiles.ContainsKey(position))
            {
                operateBoard.SetTile(position, originalTiles[position]);
            }
            else
            {
                operateBoard.SetTile(position, null);
            }
        }
    }

    private void HighlightTile(Vector3Int basePosition)
    {
        if (currentTetri == null)
        {
            return;
        }

        highlightedTiles.Clear();

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            highlightedTiles.Add(tilePosition);

            if (operateBoard.HasTile(tilePosition))
            {
                if (originalTiles.ContainsKey(tilePosition) && originalTiles[tilePosition] == setedTile)
                {
                    operateBoard.SetTile(tilePosition, forbidTile);
                }
                else
                {
                    operateBoard.SetTile(tilePosition, highlightTile);
                }
            }
            else
            {
                operateBoard.SetTile(tilePosition, forbidTile);
            }
        }
    }

    private void SetTileAndNeighbors(Vector3Int basePosition, TileBase tile)
    {
        if (currentTetri == null)
        {
            return;
        }

        // 检查当前瓦片及其相邻瓦片是否有 forbidTile
        if (IsAnyTileForbidden(basePosition))
        {
            Debug.Log("Cannot set tiles because one or more tiles are forbidden.");
            return;
        }

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            SetTile(tilePosition, tile);
        }
    }

    private bool IsAnyTileForbidden(Vector3Int basePosition)
    {
        if (currentTetri == null)
        {
            return false;
        }

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            if (operateBoard.GetTile(tilePosition) == forbidTile)
            {
                return true;
            }
        }

        return false;
    }

    private void SetTile(Vector3Int position, TileBase tile)
    {
        if (operateBoard.HasTile(position))
        {
            originalTiles[position] = tile;
            operateBoard.SetTile(position, tile);
            
        }
    }

    public void OnSettlementConfirm()
    {
        BoundsInt bounds = operateBoard.cellBounds;
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            bool allSeted = true;
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (operateBoard.GetTile(position) != setedTile)
                {
                    allSeted = false;
                    break;
                }
            }

            if (allSeted)
            {
                // TODO: 执行一些操作
                Debug.Log("Row " + y + " is fully set.");
            }
            else 
            {
                Debug.Log("Row " + y + " is not fully set.");
            }
        }

        // 将所有瓦片设置回 normalTile 状态
        foreach (var kvp in originalTiles)
        {
            operateBoard.SetTile(kvp.Key, normalTile);
        }

        InitializeOriginalTiles();

    }

    // 获取与指定位置相连的所有瓦片
    private Tetri selectTetri(Vector3Int startPos)
    {
        Tetri tetri = new Tetri(startPos);
        Queue<Vector3Int> toCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> checkedPositions = new HashSet<Vector3Int>();

        toCheck.Enqueue(startPos);

        while (toCheck.Count > 0)
        {
            Vector3Int currentPos = toCheck.Dequeue();
            if (checkedPositions.Contains(currentPos))
            {
                continue;
            }

            if (stuffBoard.HasTile(currentPos))
            {
                tetri.AddTile(currentPos);
                checkedPositions.Add(currentPos);

                Vector3Int[] neighbors = new Vector3Int[]
                {
                    currentPos + Vector3Int.up,
                    currentPos + Vector3Int.down,
                    currentPos + Vector3Int.left,
                    currentPos + Vector3Int.right
                };

                foreach (var neighbor in neighbors)
                {
                    if (!checkedPositions.Contains(neighbor))
                    {
                        toCheck.Enqueue(neighbor);
                    }
                }
            }
        }

        return tetri;
    }
}
