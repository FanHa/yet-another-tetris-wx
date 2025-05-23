
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Cell : MonoBehaviour {
        [SerializeField] private Image image; // 用于显示单元格的图像
        private Outline outline; // 用于显示单元格的轮廓

        private void Awake() {
            outline = GetComponent<Outline>(); // 获取轮廓组件
        }
        public void SetImage(Sprite sprite) {
            if (image != null) {
                image.sprite = sprite; // 设置图像
            }
        }

        public void SetTransparent() {
            Image selfImage = GetComponent<Image>();
            if (selfImage != null) {
                Color color = selfImage.color;
                color.a = 0f; // 设置 Alpha 值为 0
                selfImage.color = color;
            }
            if (image != null) {
                Color color = image.color;
                color.a = 0f; // 设置 Alpha 值为 0
                image.color = color;
            }
        }

        public void SetCellOutLineColor(Color color){
            if (outline != null) {
                outline.effectColor = color; // 设置轮廓颜色为红色
            }
        }

    }
}