// This file is part of the TetriGame project.
using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class TetriInventoryController : MonoBehaviour
    {
        [SerializeField] private AreaScrollHandler areaScrollHandler;
        [SerializeField] private Transform content;
        [SerializeField] private Transform operationTableTransform;
        private View.TetriInventoryView view;
        [SerializeField] private Model.TetriInventoryModel model;
        public event Action<Operation.Tetri> OnTetriBeginDrag; // 新增事件
        public event Action<Operation.Tetri> OnTetriClick; // 新增事件
        [SerializeField] private Operation.TetriFactory tetriFactory;

        private float minY;
        private float maxY;
        void Awake()
        {
            view = GetComponent<View.TetriInventoryView>();

        }
        void Start()
        {

            model.OnDataChanged += HandleDataChanged;
            model.Init();
        }

        public void MarkTetriAsUsed(Model.Tetri.Tetri tetri)
        {
            model.MarkTetriAsUsed(tetri);
        }


        private void HandleDataChanged()
        {
            UpdateView();
            ComputeScrollLimits();
        }

        private void ComputeScrollLimits()
        {
            float cellSize = 6f;
            int rowCount = Mathf.CeilToInt((float)model.UsableTetriList.Count / 2);
            float contentHeight = rowCount * cellSize;

            Bounds area = areaScrollHandler.GetComponent<Collider2D>().bounds;
            float visibleHeight = area.size.y;

            // 内容高度小于可见高度时，不允许滑动
            if (contentHeight <= visibleHeight)
            {
                maxY = content.localPosition.y;
                minY = content.localPosition.y;
            }
            else
            {
                maxY = content.localPosition.y + visibleHeight;
                minY = content.localPosition.y;
            }
            areaScrollHandler.SetScrollLimits(minY, maxY);
        }

        private void UpdateView()
        {
            List<GameObject> tetriList = new List<GameObject>();
            foreach (var tetriModel in model.UsableTetriList)
            {
                // 创建 Tetri 实例
                var tetriComponent = tetriFactory.CreateTetri(tetriModel);
                tetriComponent.OnBeginDragEvent += HandleTetriBeginDrag;
                tetriComponent.OnClickEvent += HandleTetriClick;

                tetriList.Add(tetriComponent.gameObject);

            }
            view.ShowItems(tetriList);
        }

        private void HandleTetriBeginDrag(Operation.Tetri tetri)
        {
            OnTetriBeginDrag?.Invoke(tetri);
        }

        internal void AddTetri(Tetri modelTetri)
        {
            model.AddTetri(modelTetri);
        }
        
        private void HandleTetriClick(Operation.Tetri tetri)
        {
            OnTetriClick?.Invoke(tetri);
        }
    }
}