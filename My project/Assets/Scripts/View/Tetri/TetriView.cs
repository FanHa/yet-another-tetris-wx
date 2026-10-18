using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TetriModel = Model.Tetri.Tetri;

namespace View.Tetri
{
    public abstract class TetriView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {

        public event Action<TetriView> OnBeginDragEvent;
        public event Action<Vector3> OnDragEvent;
        public event Action OnEndDragEvent;
        public event Action<TetriView> OnClickEvent;

        public abstract TetriModel ModelTetri { get; }
        private Camera mainCamera;
        private Vector3 dragOffset;

        void Awake()
        {
            mainCamera = Camera.main;
        }
        private void OnDestroy()
        {
        }

        protected abstract void RebuildFromModel();
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 pointerPosition = GetPointerWorldPosition(eventData.pressPosition, eventData.pressEventCamera);
            dragOffset = transform.position - pointerPosition;
            OnBeginDragEvent?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 pointerPosition = GetPointerWorldPosition(eventData.position, eventData.pressEventCamera);
            OnDragEvent?.Invoke(pointerPosition + dragOffset);
        }

        private Vector3 GetPointerWorldPosition(Vector2 screenPosition, Camera eventCamera)
        {
            Camera pointerCamera = eventCamera != null ? eventCamera : mainCamera;
            Ray pointerRay = pointerCamera.ScreenPointToRay(screenPosition);
            var dragPlane = new Plane(Vector3.forward, transform.position);
            dragPlane.Raycast(pointerRay, out float distance);
            return pointerRay.GetPoint(distance);
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

    public abstract class TetriView<TModel> : TetriView where TModel : TetriModel
    {
        public TModel Model { get; private set; }
        public sealed override TetriModel ModelTetri => Model;

        public void Initialize(TModel model)
        {
            Model = model;
            RebuildFromModel();
        }
    }
}