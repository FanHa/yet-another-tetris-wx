#if !DISABLE_OBSOLETE_CLASSES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OperateBoardManager : MonoBehaviour
{
    [SerializeField] private Tilemap tableBoard;
    [SerializeField] private Tilemap maskBoard;
    [SerializeField] private Tile validTile;
    [SerializeField] private Tile invalidTile;
    [SerializeField] private Tile setedTile;

    private Stuff draggingStuff;
    private Vector3Int lastMouseCellPos;
    private BoundsInt validBounds;
    // Start is called before the first frame update
    void Start()
    {
        lastMouseCellPos = Vector3Int.zero;
        validBounds = new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(8, 8, 1)); // 定义有效的8x8格子范围

    }

    // Update is called once per frame
    void Update()
    {
        if (draggingStuff != null )
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int currentMouseCellPos = tableBoard.WorldToCell(mouseWorldPos);

            if (currentMouseCellPos != lastMouseCellPos)
            {
                ClearMask();
                SetMask(currentMouseCellPos, draggingStuff);
                lastMouseCellPos = currentMouseCellPos;
            }
        }
    }

    public void StartDragging(Stuff stuff)
    {
        draggingStuff = stuff;
    }

    public void StopDragging()
    {
        draggingStuff = null;
        ClearMask();
    }

    public void SetStuff()
    {
        if (draggingStuff == null)
        {
            return;
        }

        // 检查当前要放置的Stuff位置是否有任何一个瓦片是禁止的
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentMouseCellPos = tableBoard.WorldToCell(mouseWorldPos);
        
        if (IsAnyTileForbidden(currentMouseCellPos, draggingStuff))
        {
            Debug.Log("Cannot set tiles because one or more tiles are forbidden.");
            return;
        }
        foreach (var offset in draggingStuff.Tetri.RelativePositions)
        {
            
            Vector3Int tilePosition = currentMouseCellPos + offset;
            // TileBase originalTile = operateBoard.GetTile(tilePosition);
            // todo 暂时设置为 setedTile，之后可能会根据 stuff 的类型来设置不同的瓦片
            SetTableTile(tilePosition, setedTile);
        }
        
        MaterialAssembly();
    }

    //
    // Summary:
    // 组装材料, 这里以后需要根据不同的规则判断是否满足规则,然后生成新兵种
    private void MaterialAssembly()
    {
        for (int y = validBounds.yMin; y < validBounds.yMax; y++)
        {
            bool isFullRow = true;

            for (int x = validBounds.xMin; x < validBounds.xMax; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (tableBoard.GetTile(tilePosition) != setedTile)
                {
                    isFullRow = false;
                    break;
                }
            }

            if (isFullRow)
            {
                ClearRow(y);
            }
        }
    }

    private void ClearRow(int y)
    {
        BoundsInt bounds = tableBoard.cellBounds;

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 0);
            tableBoard.SetTile(tilePosition, null);
        }
    }
    private bool IsAnyTileForbidden(Vector3Int basePosition, Stuff stuff)
    {
        if (stuff == null)
        {
            return false;
        }

        foreach (var offset in stuff.Tetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;

            if (!tableBoard.HasTile(tilePosition) || tableBoard.GetTile(tilePosition) == setedTile)
            {
                return true;
            }
        }

        return false;
    }

    private void SetTableTile(Vector3Int position, TileBase tile)
    {
        if (tableBoard.HasTile(position))
        {
            tableBoard.SetTile(position, tile);
            
        }
    }


    private void SetMask(Vector3Int basePosition, Stuff stuff)
    {
        if (stuff == null)
        {
            return;
        }

        // highlightedTiles.Clear();

        foreach (var offset in stuff.Tetri.RelativePositions)
        {
            Vector3Int tilePosition = basePosition + offset;

            if (tableBoard.HasTile(tilePosition))
            {
                // todo 这里以后再判断具体的tile,暂时所有setedtile都是一样的
                if (tableBoard.GetTile(tilePosition) == setedTile)
                {
                    maskBoard.SetTile(tilePosition, invalidTile);
                }
                else
                {
                    maskBoard.SetTile(tilePosition, validTile);
                }
            }
            else
            {
                maskBoard.SetTile(tilePosition, invalidTile);
            }
        }
    }

    private void ClearMask()
    {
        maskBoard.ClearAllTiles();
    }


}
#endif