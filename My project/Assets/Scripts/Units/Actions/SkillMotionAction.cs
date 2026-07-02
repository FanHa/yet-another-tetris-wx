namespace Units.Actions
{
    public sealed class SkillMotionAction : UnitAction<IStatusActionContext>
    {
        public override int Priority => 30;

        public SkillMotionAction(IStatusActionContext context) : base(context, UnitActionType.SkillMotion)
        {
        }

        public override bool CanStart()
        {
            return Context.IsSkillMotionActive;
        }

        protected override void OnEnter()
        {
            Context.PrepareForSkillMotion();
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