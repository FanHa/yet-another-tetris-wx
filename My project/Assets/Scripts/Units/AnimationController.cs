using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int CastSkillTriggerHash = Animator.StringToHash("CastSkill");
        private static readonly int DieTriggerHash = Animator.StringToHash("Die");

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
        private int animationToken;
        private bool isDead;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(animator != null, $"[AnimationController] Animator missing on {name}");
        }

        /// <summary>
        /// 类型安全入口。所有新调用应通过该方法提交动画命令。
        /// </summary>
        public AnimationResult Apply(AnimationCommand command)
        {
            if (command == null)
            {
                Debug.LogWarning($"[AnimationController] Null command ignored. unit={name}, token={animationToken}, time={Time.time:F3}");
                return new AnimationResult { token = animationToken };
            }

            if (isDead)
            {
                return new AnimationResult { token = animationToken };
            }

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
                    return new AnimationResult { token = animationToken };
                default:
                    Debug.LogWarning($"[AnimationController] Unknown command ignored. unit={name}, command={command.GetType().Name}, token={animationToken}, time={Time.time:F3}");
                    return new AnimationResult { token = animationToken };
            }
        }

        private int PlayAttackAnimation()
        {
            animationToken++;
            animator.SetTrigger(AttackTriggerHash);
            return animationToken;
        }

        private int PlayCastSkillAnimation()
        {
            animationToken++;
            animator.SetTrigger(CastSkillTriggerHash);
            return animationToken;
        }

        private int PlayDeathAnimation()
        {
            animationToken++;
            isDead = true;
            animator.SetTrigger(DieTriggerHash);
            return animationToken;
        }

        private void StopActionAnimation()
        {
            animationToken++;
            animator.ResetTrigger(AttackTriggerHash);
            animator.ResetTrigger(CastSkillTriggerHash);
        }

        public bool IsTokenCurrent(int token)
        {
            return animationToken == token;
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