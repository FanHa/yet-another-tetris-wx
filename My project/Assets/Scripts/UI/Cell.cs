
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Cell : MonoBehaviour {
        [SerializeField] private Image image; // 用于显示单元格的图像

        public void SetImage(Sprite sprite) {
            if (image != null) {
                image.sprite = sprite; // 设置图像
            }
        }
    }
}