using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    [SerializeField] private Tilemap operateBoard;
    [SerializeField] private Tilemap operateBoardMask;
    [SerializeField] private Tilemap stuffBoard;
    [SerializeField] private Tilemap stuffBoardMask;
    [SerializeField] private TileBase maskTile;
    [SerializeField] private TileBase normalTile;
    [SerializeField] private TileBase setedTile;
    [SerializeField] private TileBase forbidTile;
    [SerializeField] private TileBase stuffMaskTile;
    // private Vector3Int previousMousePos = new Vector3Int();
    private BoardState boardState;
    private Tetri currentTetri;
    private List<Vector3Int> highlightedTiles = new List<Vector3Int>();
    private bool isDragging = false;
    private Stack<List<TileOperation>> history = new Stack<List<TileOperation>>();
    private Vector3Int draggingStuffPosition;
    void Start()
    {
        InitializeBoardState();
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int operateBoardCellPos = operateBoard.WorldToCell(mouseWorldPos);
        Vector3Int stuffBoardCellPos = stuffBoard.WorldToCell(mouseWorldPos);
        // todo
        stuffBoardCellPos.z = 0;
        if (isDragging)
        {
            // 恢复之前高亮的瓦片
            // todo 这里可能会有性能问题，因为每帧都会调用 RestoreHighliteedTiles
            UnmaskTetri();

            // 检查鼠标是否在 operateBoard 上
            if (IsMouseOnOperateBoard(operateBoardCellPos))
            {
                // 根据选中的 tetri 的形状来高亮瓦片
                MaskTetriToBoard(operateBoardCellPos);
            }

            // 在 stuffBoardMask 上给原来 stuff 的位置的瓦片加上 stuffMaskTile
            MaskStuffOnBoard(draggingStuffPosition);
        }

        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键按下
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == stuffBoard.gameObject)
            {
                Tetri tetri = selectTetri(stuffBoardCellPos);
                currentTetri = tetri;
                draggingStuffPosition = stuffBoardCellPos;
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) // 检测鼠标左键释放
        {
            if (isDragging)
            {
                if (IsMouseOnOperateBoard(operateBoardCellPos))
                {
                    SetTetri(operateBoardCellPos, setedTile);
                }
                isDragging = false;
                
                UnmaskTetri();
                UnmaskStuffOnBoard(draggingStuffPosition);

                currentTetri = null;
            }
        }
    }

    private void MaskStuffOnBoard(Vector3Int basePosition)
    {
        if (currentTetri == null)
        {
            return;
        }

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            stuffBoardMask.SetTile(tilePosition, stuffMaskTile);
        }
    }

    private void UnmaskStuffOnBoard(Vector3Int basePosition)
    {
        if (currentTetri == null)
        {
            return;
        }

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            stuffBoardMask.SetTile(tilePosition, null);
        }
    }

    private bool IsMouseOnOperateBoard(Vector3Int mouseCellPos)
    {
        BoundsInt bounds = operateBoard.cellBounds;
        return bounds.Contains(mouseCellPos) && operateBoard.HasTile(mouseCellPos);
    }


    private void InitializeBoardState()
    {
        // 初始化 currentTileMatrix
        BoundsInt bounds = operateBoard.cellBounds;
        TileBase[] allTiles = operateBoard.GetTilesBlock(bounds);
        Dictionary<Vector3Int, TileBase> tileDictionary = new Dictionary<Vector3Int, TileBase>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    tileDictionary[position] = tile;
                }
            }
        }

        boardState = new BoardState(tileDictionary);
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
            TileBase stuffBoardTile = stuffBoard.GetTile(draggingStuffPosition + offset);
            currentOperation.Add(new TileOperation(tilePosition, originalTile, stuffBoardTile));
            SetTile(tilePosition, tile);
        }

        history.Push(currentOperation);

        // 清除 stuffBoard 上的 tetri 瓦片
        ClearTetriFromStuffBoard(draggingStuffPosition);

        // 调用 OnSettlementConfirm
        OnSettlementConfirm();
    }

    private void ClearTetriFromStuffBoard(Vector3Int basePosition)
    {
        if (currentTetri == null)
        {
            return;
        }

        foreach (var offset in currentTetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;
            stuffBoard.SetTile(tilePosition, null);
        }
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            List<TileOperation> lastOperation = history.Pop();
            foreach (var operation in lastOperation)
            {
                operateBoard.SetTile(operation.Position, operation.OriginalTile);
                stuffBoard.SetTile(operation.Position - draggingStuffPosition, operation.StuffBoardTile);

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
            boardState.SetTile(position, tile);
            operateBoard.SetTile(position, tile);
            
        }
    }

    // todo
    public void OnSettlementConfirm()
    {
        // 获取矩阵
        TileBase[,] matrix = boardState.GetMatrix();
        List<int> rowsToReset = new List<int>();

        // 遍历每一行
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
            bool allSeted = true;

            // 遍历当前行的每一个瓦片
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                if (matrix[x, y] != setedTile)
                {
                    allSeted = false;
                    break;
                }
            }

            if (allSeted)
            {
                // 记录需要重置的行
                rowsToReset.Add(y);
                // Debug.Log("Row " + y + " is fully set.");
            }
            else
            {
                // Debug.Log("Row " + y + " is not fully set.");
            }
        }

        // 重置那些成行的方块
        foreach (int y in rowsToReset)
        {
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                Vector3Int position = new Vector3Int(x + boardState.MinX, y + boardState.MinY, 0);
                boardState.SetTile(position, normalTile);
                operateBoard.SetTile(position, normalTile);
            }
        }
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
