namespace Units.Actions
{
    public sealed class CastSkillAction : UnitAction<ISkillActionContext>
    {
        public override int Priority => 20;
        private bool hasExecuted;
        private float castSkillElapsedSeconds;

        public CastSkillAction(ISkillActionContext context) : base(context, UnitActionType.CastSkill)
        {
        }

        public override bool CanStart()
        {
            return Context.HasReadySkill && Context.ActionSpeed > 0f;
        }

        protected override void OnEnter()
        {
            Context.SuspendAutoMovement();
            hasExecuted = false;
            castSkillElapsedSeconds = 0f;
        }

        private void AdvanceCastSkillTimeline(global::Units.Actions.ActionTickContext context)
        {
            castSkillElapsedSeconds += context.DeltaTime * UnityEngine.Mathf.Max(0f, context.TimelineSpeed);
        }

        private bool HasCompletedCastSkillTimeline()
        {
            float durationSeconds = UnityEngine.Mathf.Max(0.001f, Context.GetSkillActionDurationSeconds());
            return castSkillElapsedSeconds >= durationSeconds;
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            AdvanceCastSkillTimeline(context);

            if (!HasCompletedCastSkillTimeline())
            {
                return;
            }

            if (!hasExecuted)
            {
                Context.ExecutePendingSkill();
                RaiseCommitted(UnitActionCommitKind.SkillExecuted);
                hasExecuted = true;
            }

            Complete();
        }

        protected override void OnExit()
        {
            Context.ResumeAutoMovement();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
        }
    }
}
