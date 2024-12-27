#if !DISABLE_OBSOLETE_CLASSES
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    [SerializeField] private Tilemap operateBoard;
    [SerializeField] private Tilemap operateBoardMask;
    // [SerializeField] private Tilemap stuffBoard;

    [SerializeField] private DeckManager deckManager;
    [SerializeField] private StuffBoardManager stuffBoardManager;
    [SerializeField] private OperateBoardManager operateBoardManager;



    private BoardState boardState;
    private Stuff currentStuff;

    private List<Vector3Int> highlightedTiles = new List<Vector3Int>();
    private bool isDragging = false;
    void Start()
    {
        InitializeOperateBoardState();
        RefreshStuffBoard();

    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int operateBoardCellPos = operateBoard.WorldToCell(mouseWorldPos);

        if (isDragging)
        {
            if (IsMouseOnOperateBoard(operateBoardCellPos))
            {
                operateBoardManager.StartDragging(currentStuff);
            }
            else
            {
                operateBoardManager.StopDragging();
            }
        }

        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键按下
        {
            // Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos2D, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Stuff"))
            {
                currentStuff = hit.collider.GetComponent<Stuff>();
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) // 检测鼠标左键释放
        {
            if (isDragging)
            {
                if (IsMouseOnOperateBoard(operateBoardCellPos))
                {
                    operateBoardManager.SetStuff();
                    stuffBoardManager.RemoveStuff(currentStuff);
                    deckManager.AddUsedCard(currentStuff.Tetri);
                }
                isDragging = false;
                currentStuff = null;
                operateBoardManager.StopDragging();
            }
        }
    }

    public void RefreshStuffBoard() {
        var tetris = deckManager.DrawCards(3);
        
        foreach (var tetri in tetris) 
        {
            stuffBoardManager.AddStuff(tetri);
        }
    }

    private bool IsMouseOnOperateBoard(Vector3Int mouseCellPos)
    {
        BoundsInt bounds = operateBoard.cellBounds;
        return bounds.Contains(mouseCellPos) && operateBoard.HasTile(mouseCellPos);
    }


    private void InitializeOperateBoardState()
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

}
#endif