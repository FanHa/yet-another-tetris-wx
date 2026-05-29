namespace Units.Actions
{
    public interface ISkillCastAnimationEndHandler
    {
        void HandleSkillCastAnimationEnd();
    }

    public sealed class CastSkillAction : UnitAction, ISkillCastAnimationEndHandler
    {
        public override int Priority => 20;

        public CastSkillAction(Unit owner) : base(owner, UnitActionType.CastSkill)
        {
        }

        public override bool CanStart()
        {
            return Owner.SkillHandler.HasReadySkill && Owner.Attributes.ActionSpeed.finalValue > 0f;
        }

        protected override void OnEnter()
        {
            Owner.Movement.PauseNavigation();
            Owner.AnimationController.TriggerCastSkill();
        }

        protected override void OnExit()
        {
            Owner.Movement.ResumeNavigation();
        }

        public void HandleSkillCastAnimationEnd()
        {
            if (IsCompleted)
            {
                return;
            }

            Owner.SkillHandler.ExecutePendingSkill();
            Complete();
        }
    }
}
