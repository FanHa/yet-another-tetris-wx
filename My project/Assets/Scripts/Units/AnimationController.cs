using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int CastSkillTriggerHash = Animator.StringToHash("CastSkill");
        private static readonly int DieTriggerHash = Animator.StringToHash("Die");
        private const int BaseLayerIndex = 0;



        private Animator animator;
        private bool isDead;

        // 攻击动画在 1x 速度下的基准持续时长（秒）。
        // AttackAction 用这个值初始化时间线，乘以当前 TimelineSpeed 后决定实际前摇长度。
        [SerializeField] private float attackActionDurationSeconds = 0.5f;

        // 技能动画在 1x 速度下的基准持续时长（秒）。
        // CastSkillAction 用这个值初始化时间线，乘以当前 TimelineSpeed 后决定实际前摇长度。
        [SerializeField] private float skillActionDurationSeconds = 1f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(animator != null, $"[AnimationController] Animator missing on {name}");
        }

        public void PlayAttack()
        {
            if (isDead) return;
            PlayAttackAnimation();
        }

        public void PlayCastSkill()
        {
            if (isDead) return;
            PlayCastSkillAnimation();
        }

        public void PlayDeath()
        {
            if (isDead) return;
            PlayDeathAnimation();
        }

        public void StopAction()
        {
            if (isDead) return;
            StopActionAnimation();
        }

        private void PlayAttackAnimation()
        {
            animator.SetTrigger(AttackTriggerHash);
        }

        private void PlayCastSkillAnimation()
        {
            animator.SetTrigger(CastSkillTriggerHash);
        }

        private void PlayDeathAnimation()
        {
            isDead = true;
            animator.SetTrigger(DieTriggerHash);
        }

        private void StopActionAnimation()
        {
            animator.ResetTrigger(AttackTriggerHash);
            animator.ResetTrigger(CastSkillTriggerHash);
        }

        public float GetAttackActionDurationSeconds()
        {
            return Mathf.Max(0.001f, attackActionDurationSeconds);
        }

        public float GetSkillActionDurationSeconds()
        {
            return Mathf.Max(0.001f, skillActionDurationSeconds);
        }

        public void SetPlaybackSpeed(float speedMultiplier)
        {
            animator.speed = Mathf.Max(0f, speedMultiplier);
        }

        public void ResetPlaybackSpeed()
        {
            animator.speed = 1f;
        }
    }
}