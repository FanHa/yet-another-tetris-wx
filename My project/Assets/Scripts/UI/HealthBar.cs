using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider Life;
        [SerializeField] private Slider Shield;
        private Camera healthCamera;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        private Units.Attributes attributes;

        private void Awake()
        {
            // 获取主相机的引用
            if (healthCamera == null)
            {
                healthCamera = Camera.main;
            }
        }

        public void SetAttributes(Units.Attributes attributes)
        {
            this.attributes = attributes;
            UpdateImmediate();

        }

        private void Update()
        {
            
            transform.rotation = healthCamera.transform.rotation;
            transform.position = target.position + offset;
            
            if (attributes == null) return;

            // 读取属性实时刷新
            float maxHealth = Mathf.Max(attributes.MaxHealth.finalValue, 1f);
            float currentHealth = Mathf.Clamp(attributes.CurrentHealth, 0f, maxHealth);
            float shieldValue = Mathf.Max(attributes.ShieldValue, 0f);

            Life.value = currentHealth / maxHealth;

            float shieldNormalized = Mathf.Clamp01(shieldValue / maxHealth);
            Shield.value = shieldNormalized;
            
        }
        private void UpdateImmediate()
        {
            float maxHealth = Mathf.Max(attributes.MaxHealth.finalValue, 1f);
            float currentHealth = Mathf.Clamp(attributes.CurrentHealth, 0f, maxHealth);
            float shieldValue = Mathf.Max(attributes.ShieldValue, 0f);

            if (Life != null)
                Life.value = currentHealth / maxHealth;

            if (Shield != null)
                Shield.value = Mathf.Clamp01(shieldValue / maxHealth);
        }
    }
}
