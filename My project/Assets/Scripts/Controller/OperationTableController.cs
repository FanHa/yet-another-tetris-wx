using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{

    public class OperationTableController : MonoBehaviour
    {
        public event Action<Model.Tetri.Tetri> OnTetriPlaced;
        [Header("容器大小")]
        [SerializeField] private int width;
        [SerializeField] private int height;

        [Header("每个格子的间距")]
        [SerializeField] private float cellSize;

        [SerializeField] private Model.OperationTableModel model;
        private View.OperationTableView view;

        private void Awake()
        {
            view = GetComponent<View.OperationTableView>();
            model.OnChanged += UpdateView;
            model.Clear();
        }
        private void OnDestroy()
        {
            model.OnChanged -= UpdateView;
        }
        private void Start()
        {
            view.CreateGridBackground();
        }

        private void UpdateView()
        {
            // 让 View 根据 model.PlacedTetris 刷新显示
            view.Refresh(model.PlacedTetris);
        }

        // public void OnDrop(PointerEventData eventData)
        // {
        //     var tetriObj = eventData.pointerDrag?.GetComponent<Operation.Tetri>();
        //     if (tetriObj == null) return;

        //     // 获取 Tetri 的当前位置
        //     Vector3 tetriWorldPosition = tetriObj.transform.position;

        //     // 将世界坐标转换为网格坐标
        //     Vector3 tetriLocalPosition = transform.InverseTransformPoint(tetriWorldPosition);
        //     Vector2Int gridPosition = new Vector2Int(
        //         Mathf.FloorToInt((tetriLocalPosition.x + width / 2f - 0.5f) / cellSize),
        //         Mathf.FloorToInt((height / 2f - tetriLocalPosition.y - 0.5f) / cellSize)
        //     );
        //     Operation.Tetri operationTetri = tetriObj.GetComponent<Operation.Tetri>();
        //     Model.Tetri.Tetri modelTetri = operationTetri.ModelTetri;
        //     // 尝试将 Tetri 放置到 OperationTableModel
        //     if (model.TryPlaceTetri(modelTetri, gridPosition))
        //     {
        //         OnTetriPlaced?.Invoke(modelTetri);
        //         Debug.Log("Tetri placed successfully!");

        //     }

        // }

        public bool TryPlaceTetri(Model.Tetri.Tetri modelTetri, Vector3 worldPosition)
        {
            // 将世界坐标转换为网格坐标
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
            Vector2Int gridPosition = new Vector2Int(
                Mathf.FloorToInt((localPosition.x + width / 2f - 0.5f) / cellSize),
                Mathf.FloorToInt((height / 2f - localPosition.y - 0.5f) / cellSize)
            );

            // 调用 Model 的 TryPlaceTetri 方法
            return model.TryPlaceTetri(modelTetri, gridPosition);
        }
    }

}