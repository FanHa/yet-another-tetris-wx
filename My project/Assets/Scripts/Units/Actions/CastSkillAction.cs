namespace Units.Actions
{
    public interface ISkillCastAnimationEndHandler
    {
        void HandleSkillCastAnimationEnd();
    }

    public sealed class CastSkillAction : UnitAction, ISkillCastAnimationEndHandler
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
            var result = Owner.ApplyAnimation(new AnimationController.PlayCastSkillAnimationCommand());
            animationToken = result.token;
        }

        protected override void OnExit()
        {
            Owner.ResumeNavigation();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            Owner.ApplyAnimation(new AnimationController.StopActionAnimationCommand());
        }

        public void HandleSkillCastAnimationEnd()
        {
            if (IsCompleted || !Owner.IsAnimationTokenCurrent(animationToken))
            {
                return;
            }

            Owner.ExecutePendingSkill();
            Complete();
        }
    }
}
