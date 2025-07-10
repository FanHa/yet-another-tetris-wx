using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Operation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{

    public class OperationTableController : MonoBehaviour
    {
        public event Action<Operation.Tetri> OnTetriBeginDrag;
        public event Action<List<CharacterInfluenceGroup>> OnCharacterInfluenceGroupsChanged;

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
            
        }

        private void Start()
        {
            view.Initialize();
            view.OnItemCreated += HandleTetriCreated;
            model.OnChanged += HandleModelChanged;
            model.Clear();
        }

        private void HandleTetriCreated(Operation.Tetri tetri)
        {
            tetri.OnBeginDragEvent += HandleTetriBeginDrag;
        }

        private void HandleTetriBeginDrag(Operation.Tetri tetri)
        {
            OnTetriBeginDrag?.Invoke(tetri);
        }

        private void OnDestroy()
        {
            model.OnChanged -= HandleModelChanged;
            view.OnItemCreated -= HandleTetriCreated;
        }

        private void HandleModelChanged()
        {
            // 让 View 根据 model.PlacedTetris 刷新显示
            view.Refresh(model.PlacedTetris);
            List<CharacterInfluenceGroup> groups = model.GetCharacterInfluenceGroups();
            OnCharacterInfluenceGroupsChanged?.Invoke(groups);
        }



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

        internal void RemoveTetri(Model.Tetri.Tetri modelTetri)
        {
            // 从 Model 中移除 Tetri
            // 这里假设 PlacedTetris 中的 Tetri 是唯一的
            // 如果有多个相同的 Tetri，可能需要更复杂的逻辑来确定要移除哪个
            var placedTetri = model.PlacedTetris.FirstOrDefault(pt => pt.Tetri == modelTetri);
            if (placedTetri != null)
            {
                model.RemoveTetri(placedTetri);
            }
        }
    }

}