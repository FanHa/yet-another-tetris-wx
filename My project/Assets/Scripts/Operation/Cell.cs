using UnityEngine;

namespace Operation
{
    public class Cell : MonoBehaviour
    {
        [Header("Visual Components")]
        [SerializeField] private GameObject mask;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private GameObject borderTop;
        [SerializeField] private GameObject borderBottom;
        [SerializeField] private GameObject borderLeft;
        [SerializeField] private GameObject borderRight;

        public void SetMaskColor(Color color)
        {
            if (mask != null)
            {
                var renderer = mask.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = color;
                }
            }
        }

        public void SetIcon(Sprite sprite)
        {
            if (icon != null)
            {
                icon.sprite = sprite;
                icon.enabled = (sprite != null);
            }
        }

        public void SetBorderVisibility(bool top, bool bottom, bool left, bool right)
        {
            if (borderTop != null) borderTop.SetActive(top);
            if (borderBottom != null) borderBottom.SetActive(bottom);
            if (borderLeft != null) borderLeft.SetActive(left);
            if (borderRight != null) borderRight.SetActive(right);
        }

    }
}