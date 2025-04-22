using System.Collections;
using TMPro;
using UnityEngine;

namespace Units.Damages
{
    public class DamageView : MonoBehaviour
    {
        private TextMeshProUGUI damageText;
        private float duration = 1f; // 显示时间为1秒
        private float moveUpSpeed = 0.2f; // 向上移动的速度


        void Awake()
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }
        public void Initialize(float damage, Vector3 worldPosition)
        {
            // 设置随机偏移
            float randomOffsetX = UnityEngine.Random.Range(-0.2f, 0.2f);
            float randomOffsetY = UnityEngine.Random.Range(-0.1f, 0.1f);
            Vector3 randomOffset = new(randomOffsetX, randomOffsetY, 0f);

            // 设置位置
            // transform.SetParent(canvasTransform, false);
            transform.position = worldPosition + randomOffset;

            // 设置文本
            damageText = GetComponent<TextMeshProUGUI>();
            if (damageText != null)
            {
                int roundedDamage = Mathf.RoundToInt(damage); // 将伤害值取整
                damageText.text = roundedDamage.ToString();
            }

            // 开始渐隐和销毁逻辑
            StartCoroutine(FadeAndDestroy());
        }

        private IEnumerator FadeAndDestroy()
        {
            if (damageText == null) yield break;

            Color originalColor = damageText.color;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // 渐变透明度
                damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                transform.position += new Vector3(0f, moveUpSpeed * Time.deltaTime, 0f);

                yield return null;
            }

            Destroy(gameObject); // 销毁实例
        }
    }
}
