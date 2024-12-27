using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;


namespace UI {
    public class OperationTable : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        public event Action<TetrisResourceItem, Vector3Int> OnTetriDropped; // 定义事件

        [SerializeField] private Tilemap tilemap; // Tilemap组件

        

        private Brick[,] board; // 棋盘数据


        // 注意: 这里是全量更新
        public void UpdateData(Brick[,] newBoard)
        {
            board = newBoard;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), board[x, y].Tile);
                }
            }
        }

        // public void PreviewTetri(Vector2Int startPoint, int[,] tetriShape)
        // {
        //     bool canPlace = true;

        //     // 检查是否可以放置
        //     for (int i = 0; i < tetriShape.GetLength(0); i++)
        //     {
        //         for (int j = 0; j < tetriShape.GetLength(1); j++)
        //         {
        //             if (tetriShape[i, j] != 0)
        //             {
        //                 int x = startPoint.x + i;
        //                 int y = startPoint.y + j;

        //                 if (x < 0 || x >= rows || y < 0 || y >= columns || board[x, y] != 0)
        //                 {
        //                     canPlace = false;
        //                     break;
        //                 }
        //             }
        //         }
        //         if (!canPlace)
        //         {
        //             break;
        //         }
        //     }

        //     // 预显示
        //     for (int i = 0; i < tetriShape.GetLength(0); i++)
        //     {
        //         for (int j = 0; j < tetriShape.GetLength(1); j++)
        //         {
        //             if (tetriShape[i, j] != 0)
        //             {
        //                 int x = startPoint.x + i;
        //                 int y = startPoint.y + j;

        //                 if (canPlace)
        //                 {
        //                     tilemap.SetTile(new Vector3Int(x, y, 0), occupiedTile);
        //                 }
        //                 else
        //                 {
        //                     tilemap.SetTile(new Vector3Int(x, y, 0), null); // 清除预显示
        //                 }
        //             }
        //         }
        //     }

        //     // 通知Controller数据改变
        //     if (canPlace)
        //     {
        //         // 这里可以调用Controller的方法来通知数据改变
        //         // Controller.Instance.OnTetriPreviewChanged(startPoint, tetriShape);
        //     }
        // }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrop(PointerEventData eventData)
        {
            // 检查被拖动到当前区域的物体是否是TetrisResourceItem
            GameObject draggedObject = eventData.pointerDrag;
            if (draggedObject != null)
            {
                TetrisResourceItem resourceItem = draggedObject.GetComponent<TetrisResourceItem>();
                if (resourceItem != null)
                {
                    Vector3Int cellPosition = tilemap.WorldToCell(eventData.pointerCurrentRaycast.worldPosition);

                    // 处理TetrisResourceItem被拖动到当前区域的逻辑
                    Debug.Log("TetrisResourceItem dropped in the operation table.");

                    // 触发事件，通知订阅者
                    OnTetriDropped?.Invoke(resourceItem, cellPosition);
                }
            }
            else {
                Debug.Log("Invalid object dropped in the operation table.");
            }
            
        }
    }
}
