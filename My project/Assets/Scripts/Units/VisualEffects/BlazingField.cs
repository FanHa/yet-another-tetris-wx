using UnityEngine;

namespace Units.VisualEffects
{
    public class BlazingField : MonoBehaviour
    {
        [SerializeField] private ParticleSystem fireParticle;
        private float duration = 1f;
        private float timer = 0f;
        private bool initialized = false;

        public void Initialize(float radius, float duration)
        {
            var main = fireParticle.main;
            main.startLifetime = radius;
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