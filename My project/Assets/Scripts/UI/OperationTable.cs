using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;


namespace UI {
    public class OperationTable : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        public event Action<UI.TetrisResource.TetrisResourceItem, Vector3Int> OnTetriDropped; // 定义事件

        // [SerializeField] private Tilemap baseTileMap; // Tilemap组件
        [SerializeField] private Tilemap tilemap; // Tilemap组件
        [SerializeField] private Tile baseTile;
        [SerializeField] private TetriCellTypeResourceMapping spriteMapping; // TetriCellTypeSpriteMapping实例


        private void OnEnable()
        {
            // todo 这里为什么需要加这个?
            // 重新获取Tilemap引用
            if (tilemap == null)
            {
                tilemap = GetComponent<Tilemap>();
                if (tilemap == null)
                {
                    Debug.LogError("Tilemap component not found on the GameObject.");
                }
            }
        }

        // 注意: 这里是全量更新
        public void UpdateData(Serializable2DArray<Brick> newBoard)
        {
            var board = newBoard;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    // 获取当前砖块的Cell属性
                    TetriCell cell = board[x, y].Cell;
                    if (cell != null)
                    {
                        // 根据Cell类型找到对应的Tile
                        Tile tile = spriteMapping.GetTile(cell);
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), baseTile);
                    }
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
                UI.TetrisResource.TetrisResourceItem resourceItem = draggedObject.GetComponent<UI.TetrisResource.TetrisResourceItem>();
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
