using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Operation
{
    public class Tetri : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
    {

        public event Action<Operation.Tetri> OnBeginDragEvent;
        public event Action<Vector3> OnDragEvent;
        public event Action OnEndDragEvent;

        public Model.Tetri.Tetri ModelTetri { get; private set; }
        [SerializeField] private GameObject cellPrefab;
        private Camera mainCamera;

        void Awake()
        {
            mainCamera = Camera.main;
        }
        public void Initialize(Model.Tetri.Tetri modelTetri)
        {
            ModelTetri = modelTetri;
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
                        // 设置位置（以中心点为基准）
                        float localX = i;
                        float localY = -j; 

                        cellObj.transform.localPosition = new Vector3(localX, localY, 0);


                        // 可选：根据 cell 类型设置外观
                        // cellObj.GetComponent<CellView>().SetType(cell);

                        // 可选：存储 cellObj 引用，便于后续操作
                    }
                }
            }

            
        }



        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragEvent?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(eventData.position);
            Vector3 newPosition = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);

            OnDragEvent?.Invoke(newPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragEvent?.Invoke();
        }


    }
}