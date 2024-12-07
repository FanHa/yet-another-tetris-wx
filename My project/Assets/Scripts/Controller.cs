using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    [SerializeField] private Tilemap operateBoard;
    [SerializeField] private Tilemap operateBoardMask;
    [SerializeField] private Tilemap stuffBoard;
    [SerializeField] private TileBase maskTile;
    [SerializeField] private TileBase normalTile;
    [SerializeField] private TileBase setedTile;
    [SerializeField] private TileBase forbidTile;
    // private Vector3Int previousMousePos = new Vector3Int();
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    private Tetri currentTetri;
    private List<Vector3Int> highlightedTiles = new List<Vector3Int>();
    private bool isDragging = false;
    private Stack<List<TileOperation>> history = new Stack<List<TileOperation>>();    void Start()
    {
        InitializeOriginalTiles();
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = operateBoard.WorldToCell(mouseWorldPos);

        if (isDragging)
        {
            // 恢复之前高亮的瓦片
            // todo 这里可能会有性能问题，因为每帧都会调用 RestoreHighliteedTiles
            UnmaskTetri();

            // 检查鼠标是否在 operateBoard 上
            if (IsMouseOnOperateBoard(mouseCellPos))
            {
                // 根据选中的 tetri 的形状来高亮瓦片
                MaskTetriToBoard(mouseCellPos);
            }
        }

        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键按下
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == stuffBoard.gameObject)
            {
                Tetri tetri = selectTetri(mouseCellPos);
                currentTetri = tetri;
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) // 检测鼠标左键释放
        {
            if (isDragging)
            {
                if (IsMouseOnOperateBoard(mouseCellPos))
                {
                    SetTetri(mouseCellPos, setedTile);
                }
                isDragging = false;
                currentTetri = null;
                UnmaskTetri();
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

    private void UnmaskTetri()
    {
        foreach (var tilePosition in highlightedTiles)
        {
            operateBoardMask.SetTile(tilePosition, null);
        }
        highlightedTiles.Clear();
    }

    private void MaskTetriToBoard(Vector3Int basePosition)
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
                if (operateBoard.GetTile(tilePosition) == setedTile)
                {
                    operateBoardMask.SetTile(tilePosition, forbidTile);
                }
                else
                {
                    operateBoardMask.SetTile(tilePosition, maskTile);
                }
            }
            else
            {
                operateBoardMask.SetTile(tilePosition, forbidTile);
            }
        }
    }

    private void SetTetri(Vector3Int basePosition, TileBase tile)
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

        List<TileOperation> currentOperation = new List<TileOperation>();

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            TileBase originalTile = operateBoard.GetTile(tilePosition);
            currentOperation.Add(new TileOperation(tilePosition, originalTile));
            SetTile(tilePosition, tile);
        }

        history.Push(currentOperation);
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            List<TileOperation> lastOperation = history.Pop();
            foreach (var operation in lastOperation)
            {
                operateBoard.SetTile(operation.Position, operation.OriginalTile);
            }
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
            if (operateBoardMask.GetTile(tilePosition) == forbidTile)
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

    // todo
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
