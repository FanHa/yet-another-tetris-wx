using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Operation
{
    public class Tetri : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
    {
        [SerializeField] private GameObject cellPrefab;
        private Vector3 offset;
        private bool isDragging = false;
        private Camera mainCamera;
        void Awake()
        {
            mainCamera = Camera.main;
        }
        public void Initialize(Model.Tetri.Tetri modelTetri)
        {
            var shape = modelTetri.Shape;
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var cell = shape[i, j];
                    if (cell is not Model.Tetri.Empty)
                    {
                        // 实例化 CellPrefab
                        GameObject cellObj = Instantiate(cellPrefab, this.transform);
                        // 设置位置（假设每个 cell 间隔为 1 单位，可根据实际需求调整）
                        cellObj.transform.localPosition = new Vector3(j, -i, 0);

                        // 可选：根据 cell 类型设置外观
                        // cellObj.GetComponent<CellView>().SetType(cell);

                        // 可选：存储 cellObj 引用，便于后续操作
                    }
                }
            }

            foreach (var sr in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }
        
        

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(eventData.position);
            offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(eventData.position);
            transform.position = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z) + offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 可在此处理拖拽结束后的逻辑
        }
    }
}