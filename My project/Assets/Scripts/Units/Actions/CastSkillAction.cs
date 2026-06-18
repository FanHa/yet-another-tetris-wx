using UnityEngine;

namespace Units.Actions
{
    public sealed class CastSkillAction : UnitAction, IAnimationEventHandler
    {
        public override int Priority => 20;
        private int animationToken;

        public CastSkillAction(Unit owner) : base(owner, UnitActionType.CastSkill)
        {
        }

        public override bool CanStart()
        {
            return Owner.HasReadySkill && Owner.Attributes.ActionSpeed.finalValue > 0f;
        }

        protected override void OnEnter()
        {
            Owner.PauseNavigation();
            var result = Owner.ApplyAnimationCommand(new AnimationController.PlayCastSkillAnimationCommand());
            animationToken = result.token;
        }

        protected override void OnExit()
        {
            Owner.ResumeNavigation();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            Owner.ApplyAnimationCommand(new AnimationController.StopActionAnimationCommand());
        }

        public void HandleAnimationEvent(AnimationEventType eventType)
        {
            if (eventType != AnimationEventType.CastSkillEnd)
            {
                Debug.LogWarning($"[CastSkillAction] Unexpected animation event ignored. unit={Owner.name}, event={eventType}, expected={AnimationEventType.CastSkillEnd}, token={animationToken}, time={Time.time:F3}");
                return;
            }

            if (IsCompleted)
            {
                return;
            }

            if (!Owner.IsAnimationTokenCurrent(animationToken))
            {
                Debug.LogWarning($"[CastSkillAction] Stale animation event ignored. unit={Owner.name}, event={eventType}, expectedToken={animationToken}, time={Time.time:F3}");
                return;
            }

            Owner.ExecutePendingSkill();
            Complete();
        }
    }
}
