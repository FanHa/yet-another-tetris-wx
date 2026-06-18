using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int CastSkillTriggerHash = Animator.StringToHash("CastSkill");
        private static readonly int DieTriggerHash = Animator.StringToHash("Die");

        public enum UnitAnimationState
        {
            Idle,
            Attack,
            CastSkill,
            Death
        }

        public struct AnimationResult
        {
            public int token;
        }

        public abstract class AnimationCommand
        {
        }

        public sealed class PlayAttackAnimationCommand : AnimationCommand
        {
            public Vector2? LookDirection { get; }

            public PlayAttackAnimationCommand(Vector2? lookDirection = null)
            {
                LookDirection = lookDirection;
            }
        }

        public sealed class PlayCastSkillAnimationCommand : AnimationCommand
        {
            public PlayCastSkillAnimationCommand()
            {
            }
        }

        public sealed class PlayDeathAnimationCommand : AnimationCommand
        {
            public PlayDeathAnimationCommand()
            {
            }
        }

        public sealed class StopActionAnimationCommand : AnimationCommand
        {
            public StopActionAnimationCommand()
            {
            }
        }

        private Animator animator;
        private int animationVersion;
        private UnitAnimationState currentState = UnitAnimationState.Idle;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 类型安全入口。所有新调用应通过该方法提交动画命令。
        /// </summary>
        public AnimationResult Apply(AnimationCommand command)
        {
            switch (command)
            {
                case PlayAttackAnimationCommand attackCommand:
                    return new AnimationResult { token = PlayAttackAnimation() };
                case PlayCastSkillAnimationCommand:
                    return new AnimationResult { token = PlayCastSkillAnimation() };
                case PlayDeathAnimationCommand:
                    return new AnimationResult { token = PlayDeathAnimation() };
                case StopActionAnimationCommand:
                    StopActionAnimation();
                    return new AnimationResult { token = animationVersion };
                default:
                    Debug.LogWarning($"[AnimationController] Unknown animation command type on {name}: {command?.GetType().Name ?? "<null>"}");
                    return new AnimationResult { token = animationVersion };
            }
        }

        private int PlayAttackAnimation()
        {
            // 死亡是终态，动作动画请求不能覆盖死亡表现。
            if (currentState == UnitAnimationState.Death)
            {
                return animationVersion;
            }

            animationVersion++;

            currentState = UnitAnimationState.Attack;
            animator.SetTrigger(AttackTriggerHash);

            return animationVersion;
        }

        private int PlayCastSkillAnimation()
        {
            if (currentState == UnitAnimationState.Death)
            {
                return animationVersion;
            }

            animationVersion++;
            currentState = UnitAnimationState.CastSkill;
            animator.SetTrigger(CastSkillTriggerHash);

            return animationVersion;
        }

        private int PlayDeathAnimation()
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

        private void StopActionAnimation()
        {
            if (currentState == UnitAnimationState.Death)
            {
                return;
            }

            animationVersion++;
            currentState = UnitAnimationState.Idle;

            animator.ResetTrigger(AttackTriggerHash);
            animator.ResetTrigger(CastSkillTriggerHash);
        }

        public bool IsTokenCurrent(int token)
        {
            return animationVersion == token;
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