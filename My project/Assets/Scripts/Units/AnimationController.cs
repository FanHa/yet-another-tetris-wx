using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int CastSkillTriggerHash = Animator.StringToHash("CastSkill");
        private static readonly int DieTriggerHash = Animator.StringToHash("Die");
        private static readonly int IdleStateHash = Animator.StringToHash("IdleSwing");

        public enum ActionAnimationKind
        {
            Attack,
            CastSkill
        }

        public enum UnitAnimationState
        {
            Idle,
            Attack,
            CastSkill,
            Death
        }

        // 动画请求统一接口
        public enum AnimationMode
        {
            PlayAction,   // 播放动作动画（Attack 或 CastSkill）
            PlayDeath,    // 播放死亡动画
            StopAction    // 停止当前动作回到Idle
        }

        public struct AnimationRequest
        {
            public AnimationMode mode;
            public ActionAnimationKind? actionKind;      // 仅在 PlayAction 时使用
            public Vector2? lookDirection;               // 可选的看向方向
        }

        public struct AnimationResult
        {
            public int version;
        }

        private Animator animator;
        private int animationVersion;
        private UnitAnimationState currentState = UnitAnimationState.Idle;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 应用动画请求（统一动画控制入口）。
        /// </summary>
        public AnimationResult ApplyAnimation(AnimationRequest request)
        {
            switch (request.mode)
            {
                case AnimationMode.PlayAction:
                    if (request.actionKind.HasValue)
                    {
                        return new AnimationResult { version = PlayActionAnimation(request.actionKind.Value, request.lookDirection) };
                    }
                    break;
                case AnimationMode.PlayDeath:
                    return new AnimationResult { version = PlayDeathAnimation() };
                case AnimationMode.StopAction:
                    StopActionAnimation();
                    return new AnimationResult { version = animationVersion };
            }
            return new AnimationResult { version = animationVersion };
        }

        public int PlayActionAnimation(ActionAnimationKind kind, Vector2? lookDirection = null)
        {
            // 死亡是终态，动作动画请求不能覆盖死亡表现。
            if (currentState == UnitAnimationState.Death)
            {
                return animationVersion;
            }

            animationVersion++;

            if (lookDirection.HasValue)
            {
                float angle = Mathf.Atan2(lookDirection.Value.y, lookDirection.Value.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            switch (kind)
            {
                case ActionAnimationKind.Attack:
                    currentState = UnitAnimationState.Attack;
                    animator.SetTrigger(AttackTriggerHash);
                    break;
                case ActionAnimationKind.CastSkill:
                    currentState = UnitAnimationState.CastSkill;
                    animator.SetTrigger(CastSkillTriggerHash);
                    break;
            }

            return animationVersion;
        }

        public int PlayDeathAnimation()
        {
            if (currentState == UnitAnimationState.Death)
            {
                return animationVersion;
            }

            animationVersion++;
            currentState = UnitAnimationState.Death;
            animator.SetTrigger(DieTriggerHash);
            return animationVersion;
        }

        public void StopActionAnimation()
        {
            if (currentState == UnitAnimationState.Death)
            {
                return;
            }

            animationVersion++;
            currentState = UnitAnimationState.Idle;

            animator.ResetTrigger(AttackTriggerHash);
            animator.ResetTrigger(CastSkillTriggerHash);
            animator.Play(IdleStateHash, 0, 0f);
        }

        public bool IsAnimationVersionCurrent(int version)
        {
            return animationVersion == version;
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