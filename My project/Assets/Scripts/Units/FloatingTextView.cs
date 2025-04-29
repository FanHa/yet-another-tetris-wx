using System.Collections;
using TMPro;
using UnityEngine;

namespace Units.UI
{
    public class FloatingTextView : MonoBehaviour
    {
        private TextMeshProUGUI textComponent;
        [SerializeField] private float duration; // 默认显示时间
        [SerializeField] private float moveUpSpeed; // 默认向上移动速度

        void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        public void Initialize(string text, Vector3 worldPosition)
        {
            // 设置随机偏移
            float randomOffsetX = UnityEngine.Random.Range(-0.2f, 0.2f);
            float randomOffsetY = UnityEngine.Random.Range(-0.1f, 0.1f);
            Vector3 randomOffset = new(randomOffsetX, randomOffsetY, 0f);

            // 设置位置
            transform.position = worldPosition + randomOffset;

            // 设置文本
            if (textComponent != null)
            {
                textComponent.text = text;
            }
            // 开始渐隐和销毁逻辑
            StartCoroutine(FadeAndDestroy());
        }

        private IEnumerator FadeAndDestroy()
        {
            if (textComponent == null) yield break;

            Color originalColor = textComponent.color;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // 渐变透明度
                textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                transform.position += new Vector3(0f, moveUpSpeed * Time.deltaTime, 0f);

                yield return null;
            }

            Destroy(gameObject); // 销毁实例
        }
    }
}