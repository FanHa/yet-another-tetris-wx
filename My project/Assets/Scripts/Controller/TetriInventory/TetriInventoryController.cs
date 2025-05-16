// This file is part of the TetriGame project.
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class TetriInventoryController : MonoBehaviour
    {
        [SerializeField] private AreaScrollHandler areaScrollHandler;
        [SerializeField] private Transform content;
        private View.TetriInventoryView view;
        [SerializeField] private Model.TetriInventoryModel model;
        private float minY;
        private float maxY;
        void Awake()
        {
            view = GetComponent<View.TetriInventoryView>();
            model.OnDataChanged += HandleDataChanged;
            model.Init();

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
            view.ShowItems(model.UsableTetriList);
        }


    }
}