using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Operation
{
    public abstract class Tetri : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {

        public event Action<Operation.Tetri> OnBeginDragEvent;
        public event Action<Vector3> OnDragEvent;
        public event Action OnEndDragEvent;
        public event Action<Operation.Tetri> OnClickEvent;

        public Model.Tetri.Tetri ModelTetri { get; private set; }
        private Camera mainCamera;

        void Awake()
        {
            mainCamera = Camera.main;
        }
        public void Initialize(Model.Tetri.Tetri modelTetri)
        {
            ModelTetri = modelTetri;
            ModelTetri.OnRotated += OnModelRotated;
            RebuildFromModel();
        }

        private void OnDestroy()
        {
            if (ModelTetri != null)
                ModelTetri.OnRotated -= OnModelRotated;
        }

        private void OnModelRotated()
        {
            // 仅根据当前模型重建显示
            RebuildFromModel();
        }

        protected abstract void RebuildFromModel();
    
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
            TriggerClick();
        }

        protected void TriggerClick()
        {
            OnClickEvent?.Invoke(this);
        }


    }
}