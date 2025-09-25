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
        public event Action<Operation.Tetri> OnTetriClick;

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
            tetri.OnClickEvent += HandleTetriClick;
        }

        private void HandleTetriBeginDrag(Operation.Tetri tetri)
        {
            OnTetriBeginDrag?.Invoke(tetri);
        }

        private void HandleTetriClick(Operation.Tetri tetri)
        {
            OnTetriClick?.Invoke(tetri);
        }

        private void OnDestroy()
        {
            model.OnChanged -= HandleModelChanged;
            view.OnItemCreated -= HandleTetriCreated;
        }

        private void HandleModelChanged()
        {
            // 让 View 根据 model.PlacedTetris 刷新显示
            view.Refresh(model.PlacedMap);
            List<CharacterInfluenceGroup> groups = model.GetCharacterInfluenceGroups();
            OnCharacterInfluenceGroupsChanged?.Invoke(groups);
        }

        public CharacterInfluenceGroup GetCharacterInfluenceGroupByTetri(Model.Tetri.Tetri tetri)
        {
            return model.GetCharacterInfluenceGroupOf(tetri);
        }



        public bool TryPlaceTetri(Model.Tetri.Tetri modelTetri, Vector3 worldPosition)
        {
            // 先将世界坐标转换为操作表的本地坐标
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
            localPosition.x -= 1.5f * cellSize;
            localPosition.y += 1.5f * cellSize;

            // 计算网格左上角 (0,0) 在本地坐标中的位置
            Vector3 gridOriginLocal = new Vector3(-width / 2f + 0.5f, height / 2f - 0.5f, 0);

            // 以左上角为基准，计算 localPosition 到 gridOriginLocal 的偏移
            Vector3 offset = localPosition - gridOriginLocal;



            // 转换为网格坐标
            int gridX = Mathf.FloorToInt(offset.x / cellSize);
            int gridY = Mathf.FloorToInt(-offset.y / cellSize); // Y轴向下为正

            Vector2Int gridPosition = new Vector2Int(gridX, gridY);

            // 调用 Model 的 TryPlaceTetri 方法
            return model.TryPlaceTetri(modelTetri, gridPosition);
        }

        internal void RemoveTetri(Model.Tetri.Tetri modelTetri)
        {
            model.RemoveTetri(modelTetri);
        }
    }

}