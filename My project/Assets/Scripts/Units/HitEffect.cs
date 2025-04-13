using System.Collections;
using UnityEngine;

namespace Units
{
    public class HitEffect : MonoBehaviour
    {
        [Header("Required")]
        [SerializeField] private SpriteRenderer bodySpriteRenderer;

        [Header("Color Flash")]
        [SerializeField] private Color hitColor = Color.red;

        [Header("Scale Punch")]
        [SerializeField] private float scaleFactor = 0.9f;
        [SerializeField] private float scaleDuration = 0.1f;

        [Header("Shake")]
        [SerializeField] private float shakeStrength = 0.3f;
        [SerializeField] private float shakeDuration = 0.1f;

        [Header("Particle / Ghost")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject ghostPrefab;

        [Header("Audio")]
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioSource audioSource;

        private Color originalColor;
        private Vector3 originalScale;
        private Vector3 originalPosition;

        private void Awake()
        {
            
        }

        private void Start()
        {
            originalColor = bodySpriteRenderer.color;
            originalScale = bodySpriteRenderer.transform.localScale;
            originalPosition = bodySpriteRenderer.transform.localPosition;
       
        }

        public void PlayAll()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            StartCoroutine(HitFeedbackRoutine());

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            if (ghostPrefab != null && bodySpriteRenderer != null)
            {
                GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
                ghost.transform.localScale = transform.localScale;

                SpriteRenderer ghostRenderer = ghost.GetComponent<SpriteRenderer>();
                if (ghostRenderer != null)
                {
                    ghostRenderer.sprite = bodySpriteRenderer.sprite;
                    ghostRenderer.color = bodySpriteRenderer.color;
                }
            }

            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }

        private IEnumerator HitFeedbackRoutine()
        {
            float elapsed = 0f;
            bool isFlashing = true;

            bodySpriteRenderer.color = hitColor;
            bodySpriteRenderer.transform.localScale = originalScale * scaleFactor;

            while (elapsed < shakeDuration)
            {
                // 颜色闪烁逻辑
                if (isFlashing)
                {
                    bodySpriteRenderer.color = hitColor;
                }
                else
                {
                    bodySpriteRenderer.color = originalColor;
                }
                isFlashing = !isFlashing;

                // 震动逻辑
                Vector2 offset = Random.insideUnitCircle * shakeStrength;
                bodySpriteRenderer.transform.localPosition = originalPosition + (Vector3)offset;

                elapsed += Time.deltaTime;

                // 控制闪烁频率
                yield return null;
            }

            // 恢复原状
            bodySpriteRenderer.color = originalColor;
            bodySpriteRenderer.transform.localScale = originalScale;
            bodySpriteRenderer.transform.localPosition = originalPosition;
      
        }
    }
}