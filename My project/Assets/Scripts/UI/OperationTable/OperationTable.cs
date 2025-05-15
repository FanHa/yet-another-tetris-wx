using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


namespace UI {
    public class OperationTable : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        public event Action<UI.Resource.ItemSlot, Vector2Int> OnTetriDrop; // 定义事件

        [SerializeField] private GridLayoutGroup container; // 用于布局的GridLayoutGroup
        [SerializeField] private GameObject CellPrefab;
        [SerializeField] private GameObject CharacterHaloPrefab; // 角色光环预制件
        [SerializeField] private TetriCellTypeResourceMapping spriteMapping; // TetriCellTypeSpriteMapping实例


        /// <summary>
        /// 用于UI刷新，传入所有已放置的Tetri，自动渲染到网格上
        /// </summary>
        public void UpdateData(List<Model.PlacedTetri> placedTetris, int rows, int cols)
        {
            // 清空现有单元格
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }

            // 构建一个临时棋盘用于渲染
            var board = new Model.Tetri.Cell[rows, cols];
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < cols; y++)
                    board[x, y] = new Empty();

            // 将所有PlacedTetri的Cell放到棋盘上
            foreach (var placed in placedTetris)
            {
                var shape = placed.Tetri.Shape;
                for (int i = 0; i < shape.GetLength(0); i++)
                {
                    for (int j = 0; j < shape.GetLength(1); j++)
                    {
                        var cell = shape[i, j];
                        if (cell is not Empty)
                        {
                            int x = placed.Position.x + i;
                            int y = placed.Position.y + j;
                            if (x >= 0 && x < rows && y >= 0 && y < cols)
                                board[x, y] = cell;
                        }
                    }
                }
            }

            // 渲染到UI
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    var cellData = board[x, y];
                    GameObject newCell = Instantiate(CellPrefab, container.transform);
                    UI.Cell cellComponent = newCell.GetComponent<UI.Cell>();
                    if (cellData is not Empty)
                    {
                        if (cellComponent != null)
                            cellComponent.SetImage(spriteMapping.GetSprite(cellData));
                        if (cellData is Model.Tetri.Character)
                            Instantiate(CharacterHaloPrefab, newCell.transform);
                    }
                    else
                    {
                        cellComponent.SetTransparent();
                    }
                }
            }
        }


        // 更新网格数据
        public void UpdateData(Serializable2DArray<Model.Tetri.Cell> newBoard)
        {
            // 清空现有单元格
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }

            var board = newBoard;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Model.Tetri.Cell cellData = board[x, y];
                    GameObject newCell = Instantiate(CellPrefab, container.transform);
                    UI.Cell cellComponent = newCell.GetComponent<UI.Cell>();
                    if (cellData is not Empty)
                    {
                        
                        if (cellComponent != null)
                        {
                            cellComponent.SetImage(spriteMapping.GetSprite(cellData));
                        }
                        if (cellData is Model.Tetri.Character)
                        {
                            Instantiate(CharacterHaloPrefab, newCell.transform);
                        }
                    }
                    else
                    {
                        cellComponent.SetTransparent(); // 设置单元格为透明
                    }
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                throw new System.NotImplementedException();
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            GameObject draggedObject = eventData.pointerDrag;
            if (draggedObject != null)
            {
                UI.Resource.ItemSlot resourceItem = draggedObject.GetComponent<UI.Resource.ItemSlot>();
                if (resourceItem != null)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        container.GetComponent<RectTransform>(),
                        eventData.position,
                        eventData.pressEventCamera,
                        out Vector2 localPoint
                    );

                    // 获取 GridLayoutGroup 的 padding
                    RectOffset padding = container.padding;
                    // 考虑 padding 的偏移量
                    float adjustedX = localPoint.x + container.GetComponent<RectTransform>().rect.width / 2 - padding.left;
                    float adjustedY = -localPoint.y + container.GetComponent<RectTransform>().rect.height / 2 - padding.top;

                    // 计算单元格位置
                    Vector2Int cellPosition = new Vector2Int(
                        Mathf.FloorToInt(adjustedY / container.cellSize.y),
                        Mathf.FloorToInt(adjustedX / container.cellSize.x)
                    );

                    // 触发事件，通知订阅者
                    OnTetriDrop?.Invoke(resourceItem, cellPosition);
                }
            }
            else
            {
                Debug.Log("Invalid object dropped in the operation table.");
            }
        }

        public Vector2 CellSize => container.cellSize; // 暴露单元格大小

        public Vector3 GetCellCenterWorld(Vector2Int cellPosition)
        {
            Vector3 localPosition = new Vector3(
                cellPosition.x * container.cellSize.x + container.cellSize.x / 2,
                cellPosition.y * container.cellSize.y + container.cellSize.y / 2 ,
                0
            );
            return container.transform.TransformPoint(localPosition);
        }
    }
}
