using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Operation
{
    public class Tetri : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {

        public event Action<Operation.Tetri> OnBeginDragEvent;
        public event Action<Vector3> OnDragEvent;
        public event Action OnEndDragEvent;
        public event Action<Operation.Tetri> OnClickEvent;

        public Model.Tetri.Tetri ModelTetri { get; private set; }
        [SerializeField] private GameObject cellPrefab;
        // [SerializeField] private SpriteRenderer characterSpriteRenderer;
        [SerializeField] private Transform characterRoot; 
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;
        [SerializeField] private Transform cellsRoot;
        [SerializeField] private Units.UnitFactory unitFactory;
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

            Model.Tetri.Character mainCell = null;
            // 存储所有Cell对象，便于后续设置border
            var cellObjs = new Operation.Cell[rows, cols];

            float offsetX = (rows - 1) / 2f; // 对于4x4就是1.5
            float offsetY = (cols - 1) / 2f; // 对于4x4就是1.5

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var cell = shape[i, j];
                    if (cell is not Model.Tetri.Empty)
                    {
                        GameObject cellObj = Instantiate(cellPrefab, this.cellsRoot);
                        float localX = i - offsetX;
                        float localY = -j + offsetY;
                        cellObj.transform.localPosition = new Vector3(localX, localY, 0);

                        var cellComp = cellObj.GetComponent<Operation.Cell>();
                        cellComp.Init(cell);
                        cellObjs[i, j] = cellComp;

                        if (mainCell == null && cell is Model.Tetri.Character characterCell)
                        {
                            mainCell = characterCell;
                        }
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var cellComp = cellObjs[i, j];
                    if (cellComp == null) continue;

                    // 上下左右是否有相邻有效Cell
                    bool top = (j == 0) || (cellObjs[i, j - 1] == null);
                    bool bottom = (j == cols - 1) || (cellObjs[i, j + 1] == null);
                    bool left = (i == 0) || (cellObjs[i - 1, j] == null);
                    bool right = (i == rows - 1) || (cellObjs[i + 1, j] == null);

                    cellComp.SetBorderVisibility(top, bottom, left, right);
                }
            }

            if (modelTetri.Type == Model.Tetri.Tetri.TetriType.Character)
            {
                var unit = unitFactory.CreateUnit(mainCell);
                characterRoot.gameObject.SetActive(true);
                unit.transform.SetParent(characterRoot, false);
                cellsRoot.gameObject.SetActive(false);

            }
            else
            {
                characterRoot.gameObject.SetActive(false);
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
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent?.Invoke(this);
        }


    }
}