using UnityEngine;

namespace Units.VisualEffects
{
    public class WindStrikeArea : MonoBehaviour
    {
        [Header("Sprite Settings")]
        public SpriteRenderer spriteRenderer; // 关联的 SpriteRenderer
        public Vector2 fixedSize = new Vector2(1f, 1f); // 固定大小
        public float rotationSpeed = 90f; // 旋转速度（度/秒）

        private void Start()
        {
            // 设置 Sprite 的缩放比例以固定显示大小
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Vector2 spriteSize = spriteRenderer.sprite.bounds.size; // 获取图片的实际大小
                Vector3 scale = new Vector3(
                    fixedSize.x / spriteSize.x, // 计算 X 轴缩放比例
                    fixedSize.y / spriteSize.y, // 计算 Y 轴缩放比例
                    1f
                );
                spriteRenderer.transform.localScale = scale;
            }
            else
            {
                Debug.LogWarning("SpriteRenderer or Sprite is not assigned.");
            }
        }

        private void Update()
        {
            // 不断旋转 Sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }
    }
}