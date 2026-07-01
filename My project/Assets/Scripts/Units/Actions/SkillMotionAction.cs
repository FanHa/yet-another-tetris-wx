namespace Units.Actions
{
    public sealed class SkillMotionAction : UnitAction
    {
        public override int Priority => 30;

        public SkillMotionAction(IUnitActionContext context) : base(context, UnitActionType.SkillMotion)
        {
        }

        public override bool CanStart()
        {
            return Context.IsSkillMotionActive;
        }

        protected override void OnEnter()
        {
            Context.ClearNavigationPathForSkillMotion();
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            if (!Context.IsSkillMotionActive)
            {
                Complete();
            }
        }
    }
}