using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class TetriClickTest : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down on Tetri!");
        }
    }
}
