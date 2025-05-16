// AreaScrollHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class AreaScrollHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Transform content;
        private float minY;
        private float maxY;

        private Camera mainCamera;
        private Vector2 lastWorldPoint;
        private bool isDragging = false;

        void Awake()
        {
            mainCamera = Camera.main;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            lastWorldPoint = mainCamera.ScreenToWorldPoint(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            Vector2 current = mainCamera.ScreenToWorldPoint(eventData.position);
            float deltaY = current.y - lastWorldPoint.y;

            Vector3 newPos = content.localPosition + new Vector3(0, deltaY, 0);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
            content.localPosition = newPos;

            lastWorldPoint = current;
        }

        // 可加方法设置 minY/maxY
        public void SetScrollLimits(float min, float max)
        {
            minY = min;
            maxY = max;
        }
    }
}