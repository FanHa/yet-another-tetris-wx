using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int CastSkillTriggerHash = Animator.StringToHash("CastSkill");
        private static readonly int DieTriggerHash = Animator.StringToHash("Die");
        private static readonly int AttackStateShortNameHash = Animator.StringToHash("Attack");
        private static readonly int CastSkillStateShortNameHash = Animator.StringToHash("CastSkill");
        private const int BaseLayerIndex = 0;

        public enum ActionVisualType
        {
            Attack,
            CastSkill
        }

        public enum ActionVisualStatus
        {
            NotStarted,
            Playing,
            Finished
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
        private int animationToken;
        private bool isDead;
        private bool hasSeenAttackVisual;
        private bool hasSeenCastSkillVisual;

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
            hasSeenAttackVisual = false;
            hasSeenCastSkillVisual = false;
        }

        public bool IsTokenCurrent(int token)
        {
            return animationToken == token;
        }

        public ActionVisualStatus GetActionVisualStatus(ActionVisualType visualType, out float progress)
        {
            return visualType switch
            {
                ActionVisualType.Attack => EvaluateActionVisualStatus(AttackStateShortNameHash, ref hasSeenAttackVisual, out progress),
                ActionVisualType.CastSkill => EvaluateActionVisualStatus(CastSkillStateShortNameHash, ref hasSeenCastSkillVisual, out progress),
                _ => ReturnNotStarted(out progress)
            };
        }

        public bool TryGetActionVisualProgress(ActionVisualType visualType, out float progress)
        {
            int targetStateHash = visualType switch
            {
                ActionVisualType.Attack => AttackStateShortNameHash,
                ActionVisualType.CastSkill => CastSkillStateShortNameHash,
                _ => 0
            };

            return TryGetStateVisualProgress(targetStateHash, out progress);
        }

        private ActionVisualStatus EvaluateActionVisualStatus(int targetStateShortNameHash, ref bool hasSeenVisual, out float progress)
        {
            if (TryGetStateVisualProgress(targetStateShortNameHash, out progress))
            {
                hasSeenVisual = true;
                return ActionVisualStatus.Playing;
            }

            if (hasSeenVisual)
            {
                hasSeenVisual = false;
                progress = 1f;
                return ActionVisualStatus.Finished;
            }

            return ReturnNotStarted(out progress);
        }

        private static ActionVisualStatus ReturnNotStarted(out float progress)
        {
            progress = 0f;
            return ActionVisualStatus.NotStarted;
        }

        private bool TryGetStateVisualProgress(int targetStateShortNameHash, out float progress)
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
            if (currentState.shortNameHash == targetStateShortNameHash)
            {
                progress = NormalizeVisualProgress(currentState.normalizedTime);
                return true;
            }

            if (animator.IsInTransition(BaseLayerIndex))
            {
                var nextState = animator.GetNextAnimatorStateInfo(BaseLayerIndex);
                if (nextState.shortNameHash == targetStateShortNameHash)
                {
                    progress = NormalizeVisualProgress(nextState.normalizedTime);
                    return true;
                }
            }

            progress = 0f;
            return false;
        }

        private static float NormalizeVisualProgress(float normalizedTime)
        {
            if (float.IsNaN(normalizedTime) || float.IsInfinity(normalizedTime))
            {
                return 0f;
            }

            return Mathf.Clamp01(normalizedTime);
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