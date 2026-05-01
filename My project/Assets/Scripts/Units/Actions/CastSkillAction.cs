namespace Units.Actions
{
    public sealed class CastSkillAction : UnitAction
    {
        public override int Priority => 20;

        public CastSkillAction(Unit owner) : base(owner, UnitActionType.CastSkill)
        {
        }

        public override bool CanStart()
        {
            return Owner.SkillHandler.HasReadySkill;
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

        public void HandleAnimationEnd()
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
