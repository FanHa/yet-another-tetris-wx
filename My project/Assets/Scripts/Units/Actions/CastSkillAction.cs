namespace Units.Actions
{
    public interface ISkillCastAnimationEndHandler
    {
        void HandleSkillCastAnimationEnd();
    }

    public sealed class CastSkillAction : UnitAction, ISkillCastAnimationEndHandler
    {
        public override int Priority => 20;
        private int animationVersion;

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
            var result = Owner.ApplyAnimation(new AnimationController.AnimationRequest 
            { 
                mode = AnimationController.AnimationMode.PlayAction,
                actionKind = AnimationController.ActionAnimationKind.CastSkill
            });
            animationVersion = result.version;
        }

        protected override void OnExit()
        {
            Owner.ResumeNavigation();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            Owner.ApplyAnimation(new AnimationController.AnimationRequest 
            { 
                mode = AnimationController.AnimationMode.StopAction
            });
        }

        public void HandleSkillCastAnimationEnd()
        {
            if (IsCompleted || !Owner.IsAnimationVersionCurrent(animationVersion))
            {
                return;
            }

            Owner.ExecutePendingSkill();
            Complete();
        }
    }
}
