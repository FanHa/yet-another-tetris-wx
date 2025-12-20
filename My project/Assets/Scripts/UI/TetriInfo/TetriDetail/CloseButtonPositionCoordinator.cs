using UnityEngine;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class CloseButtonPositionCoordinator : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform contentPanel;

        public void PositionCloseButtonAtTopRight()
        {
            RectTransform buttonRect = closeButton.GetComponent<RectTransform>();


            RectTransform[] rects = contentPanel.GetComponentsInChildren<RectTransform>();
            bool initialized = false;
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;
            foreach (var rt in rects)
            {
                Vector3[] corners = new Vector3[4];
                rt.GetWorldCorners(corners);

                for (int i = 0; i < 4; i++)
                {
                    if (!initialized)
                    {
                        min = corners[i];
                        max = corners[i];
                        initialized = true;
                    }
                    else
                    {
                        min = Vector3.Min(min, corners[i]);
                        max = Vector3.Max(max, corners[i]);
                    }
                }
            }
            Vector3 topRightWorld = new Vector3(max.x, max.y, max.z);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), 
                RectTransformUtility.WorldToScreenPoint(null, topRightWorld),
                null, 
                out localPoint
            );

            // 设置按钮位置
            buttonRect.anchoredPosition = localPoint;
        }

    }
}