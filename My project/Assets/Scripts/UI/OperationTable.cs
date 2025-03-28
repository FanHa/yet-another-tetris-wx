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
        [SerializeField] private GameObject emptyCellPrefab; // 单元格预制体
        [SerializeField] private GameObject placedCellPrefab; // 放置的单元格预制体

        private Dictionary<Vector2Int, Image> cellImages = new Dictionary<Vector2Int, Image>(); // 存储单元格的Image组件


        [SerializeField] private TetriCellTypeResourceMapping spriteMapping; // TetriCellTypeSpriteMapping实例


        // 更新网格数据
        public void UpdateData(Serializable2DArray<Cell> newBoard)
        {
            // 清空现有单元格
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }
            cellImages.Clear();

            var board = newBoard;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Cell cell = board[x, y];
                    GameObject newCell;

                    if (cell is not Empty)
                    {
                        // 创建 placedCell
                        newCell = Instantiate(placedCellPrefab, container.transform);
                        Image placedCellImage = newCell.GetComponent<Image>();
                        if (placedCellImage != null)
                        {
                            placedCellImage.sprite = spriteMapping.GetSprite(cell);
                            cellImages[new Vector2Int(x, y)] = placedCellImage;
                        }
                    }
                    else
                    {
                        // 创建 emptyCell
                        newCell = Instantiate(emptyCellPrefab, container.transform);
                        Image emptyCellImage = newCell.GetComponent<Image>();
                        if (emptyCellImage != null)
                        {
                            emptyCellImage.sprite = null; // 设置为空
                            cellImages[new Vector2Int(x, y)] = emptyCellImage;
                        }
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
                        Mathf.FloorToInt(adjustedY / container.cellSize.y), // Y 轴方向需要反转,
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
                cellPosition.y * container.cellSize.y + container.cellSize.y / 2,
                0
            );
            return container.transform.TransformPoint(localPosition);
        }
    }
}
