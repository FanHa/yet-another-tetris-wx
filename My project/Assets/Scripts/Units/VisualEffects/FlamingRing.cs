using UnityEngine;

namespace Units.VisualEffects
{
    public class FlamingRing : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ringParticle;
        private Transform targetTransform;
        private bool initialized = false;

        public void Initialize(Transform target, float radius)
        {
            targetTransform = target;
            var shape = ringParticle.shape;
            shape.radius = radius;
            initialized = true;
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

            // 跟随目标
            transform.position = targetTransform.position;
        }
    }
}