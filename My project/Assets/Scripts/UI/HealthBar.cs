using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        private Camera healthCamera;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        private void Awake()
        {
            // 获取主相机的引用
            if (healthCamera == null)
            {
                healthCamera = Camera.main;
            }
        }

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            slider.value = currentHealth / maxHealth;
        }

        private void Update()
        {
            transform.rotation = healthCamera.transform.rotation;
            transform.position = target.position + offset;
        }
    }
}
