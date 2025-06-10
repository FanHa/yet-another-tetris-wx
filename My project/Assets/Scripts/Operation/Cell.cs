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
        [SerializeField] private Model.Tetri.TetriCellTypeResourceMapping resourceMapping;
        [SerializeField] private Model.Tetri.ColorConfig colorConfig; // 新增：颜色配置

        public void Init(Model.Tetri.Cell modelCell)
        {
            Sprite sprite = resourceMapping.GetSprite(modelCell);
            if (sprite != null)
            {
                icon.sprite = sprite;
            }

            var colorEntry = colorConfig.GetColorEntry(modelCell.Affinity);
            if (colorEntry != null)
            {
                SetMaskColor(colorEntry.maskColor);
                SetBorderColor(colorEntry.borderColor);
            }

        }

        private void SetMaskColor(Color color)
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

        private void SetBorderColor(Color color)
        {
            if (borderTop != null) borderTop.GetComponent<SpriteRenderer>().color = color;
            if (borderBottom != null) borderBottom.GetComponent<SpriteRenderer>().color = color;
            if (borderLeft != null) borderLeft.GetComponent<SpriteRenderer>().color = color;
            if (borderRight != null) borderRight.GetComponent<SpriteRenderer>().color = color;
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