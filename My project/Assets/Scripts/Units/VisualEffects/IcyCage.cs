using UnityEngine;

namespace Units.VisualEffects
{
    public class IcyCage : MonoBehaviour
    {
        private Transform targetTransform;
        private bool initialized = false;
        private float duration = 1f;
        private float timer = 0f;

        public void Initialize(Transform target, float duration)
        {
            targetTransform = target;
            this.duration = duration;
            timer = 0f;
            initialized = true;
            // 可选：设置图片初始位置
            if (targetTransform != null)
                transform.position = targetTransform.position;
        }

        void Update()
        {
            if (!initialized)
                return;

            if (targetTransform == null)
            {
                Destroy(gameObject);
                return;
            }

            // 始终跟随目标
            transform.position = targetTransform.position;

            timer += Time.deltaTime;
            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}