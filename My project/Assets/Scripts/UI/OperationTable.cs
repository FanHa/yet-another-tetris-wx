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

        // private Brick[,] board; // 棋盘数据


        // 注意: 这里是全量更新
        public void UpdateData(Serializable2DArray<Brick> newBoard)
        {
            var board = newBoard;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), board[x, y].Tile);
                }
            }
        }

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
