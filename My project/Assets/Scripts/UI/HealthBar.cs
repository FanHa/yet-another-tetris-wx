using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider lifeSlider;
        [SerializeField] private Slider shieldSlider;
        private Camera healthCamera;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        private Units.Attributes attributes;

        private void Awake()
        {
            healthCamera = Camera.main;

            Canvas canvas = GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = healthCamera;
            }
            
        }

        public void SetAttributes(Units.Attributes attributes)
        {
            this.attributes = attributes;
            RefreshValues();

        }

        private void Update()
        {
            transform.rotation = healthCamera.transform.rotation;
            transform.position = target.position + offset;

            if (attributes == null) return;

            RefreshValues();

        }
        
        private void RefreshValues()
        {
            if (attributes == null) return;

            float maxHealth = Mathf.Max(attributes.MaxHealth.finalValue, 1f);
            float currentHealth = Mathf.Clamp(attributes.CurrentHealth, 0f, maxHealth);
            float shieldValue = Mathf.Max(attributes.ShieldValue, 0f);
            float shieldNormalized = Mathf.Clamp01(shieldValue / maxHealth);

            lifeSlider.value = currentHealth / maxHealth;


            bool showShield = shieldNormalized > 0f;
            if (shieldSlider.gameObject.activeSelf != showShield)
                shieldSlider.gameObject.SetActive(showShield);
            if (showShield)
                shieldSlider.value = shieldNormalized;

        }
    }
}
