// This file is part of the TetriGame project.
using System;
using UnityEngine;

namespace Controller
{
    public class TetriInventoryController : MonoBehaviour
    {
        private Camera mainCamera;
        [SerializeField] private Collider2D backgroundAreaCollider;
        [SerializeField] private Transform content;
        private View.TetriInventoryView view;
        [SerializeField] private Model.TetriInventoryModel model;
        private Vector2 lastWorldPoint;
        private float minY;
        private float maxY;
        private bool isDragging = false;
        void Awake()
        {
            mainCamera = Camera.main;
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

            Bounds area = backgroundAreaCollider.bounds;
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
        }

        private void UpdateView()
        {
            view.ShowItems(model.UsableTetriList); 
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (backgroundAreaCollider == Physics2D.OverlapPoint(worldPoint))
                {
                    isDragging = true;
                    lastWorldPoint = worldPoint;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector2 current = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                float deltaY = current.y - lastWorldPoint.y;

                Vector3 newPos = content.localPosition + new Vector3(0, deltaY, 0);
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
                content.localPosition = newPos;

                lastWorldPoint = current;
            }
        }
    }
}