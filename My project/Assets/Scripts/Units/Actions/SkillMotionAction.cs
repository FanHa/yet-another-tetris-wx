namespace Units.Actions
{
    public sealed class SkillMotionAction : UnitAction
    {
        public override int Priority => 30;

        public SkillMotionAction(Unit owner) : base(owner, UnitActionType.SkillMotion)
        {
        }

        public override bool CanStart()
        {
            return Owner.IsSkillMotionActive;
        }

        protected override void OnEnter()
        {
            Owner.ClearNavigationPathForSkillMotion();
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            if (!Owner.IsSkillMotionActive)
            {
                Complete();
            }
        }
    }
}