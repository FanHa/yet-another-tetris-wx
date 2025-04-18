// FadeAndDestroy.cs
using UnityEngine;


namespace Units.Projectiles
{
    public class FadeAndDestroy : MonoBehaviour
    {
        public float lifetime = 0.3f;
        private float fadeSpeed;
        private SpriteRenderer sr;
        private Color originalColor;

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            originalColor = sr.color;
            fadeSpeed = originalColor.a / lifetime;
        }

        void Update()
        {
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            if (c.a <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                sr.color = c;
            }
        }
    }
}
