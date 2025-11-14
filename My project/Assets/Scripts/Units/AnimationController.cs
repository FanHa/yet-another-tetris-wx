using UnityEngine;

namespace Units
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on the GameObject.");
            }
        }

        public void TriggerAttack()
        {
            animator.SetTrigger("Attack");
        }

        public void TriggerCastSkill()
        {
            animator.SetTrigger("CastSkill");
        }

        public void TriggerDie()
        {
            animator.SetTrigger("Die");
        }

        public void SetLookDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}