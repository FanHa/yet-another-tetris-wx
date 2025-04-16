using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DragBlocker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            eventData.Use(); // 阻止事件传递
        }

        public void OnDrag(PointerEventData eventData)
        {
            eventData.Use(); // 同样阻止传递
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            eventData.Use(); // 保险起见
        }
    }
}

