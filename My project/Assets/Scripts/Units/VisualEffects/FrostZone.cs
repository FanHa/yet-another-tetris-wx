using UnityEngine;

namespace Units.VisualEffects
{
    public class FrostZone : MonoBehaviour
    {
        [SerializeField] private ParticleSystem edgeParticle;
        [SerializeField] private ParticleSystem centerParticle;
        private float duration = 1f;
        private float timer = 0f;
        private bool initialized = false;

        public void Initialize(float radius, float duration)
        {
            var edgeShape = edgeParticle.shape;
            edgeShape.radius = radius;
            var centerShape = centerParticle.shape;
            centerShape.radius = radius;
            this.duration = duration;
            timer = 0f;
            initialized = true;
        }

        void Update()
        {
            if (!initialized)
                return;
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}