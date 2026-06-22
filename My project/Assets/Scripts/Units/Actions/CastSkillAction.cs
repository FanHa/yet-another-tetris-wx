using UnityEngine;

namespace Units.Actions
{
    public sealed class CastSkillAction : UnitAction
    {
        public override int Priority => 20;
        private bool hasExecuted;

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
            Owner.ApplyAnimationCommand(new AnimationController.PlayCastSkillAnimationCommand());
            hasExecuted = false;
        }

        protected override void OnTick()
        {
            var visualStatus = Owner.GetActionVisualStatus(AnimationController.ActionVisualType.CastSkill, out float progress);

            if (visualStatus == AnimationController.ActionVisualStatus.Playing)
            {
                if (!hasExecuted && progress >= Owner.GetSkillCastCommitPhase())
                {
                    Owner.ExecutePendingSkill();
                    hasExecuted = true;
                }

                return;
            }

            if (visualStatus == AnimationController.ActionVisualStatus.NotStarted)
            {
                return;
            }

            if (!hasExecuted)
            {
                Owner.ExecutePendingSkill();
                hasExecuted = true;
            }

            Complete();
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
    }
}
