namespace Units.Actions
{
    public sealed class CastSkillAction : UnitAction
    {
        public override int Priority => 20;
        private bool hasExecuted;
        private readonly ActionTimelineProgress castSkillTimeline = new();

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
            hasExecuted = false;
            castSkillTimeline.Reset();
        }

        private float GetCastSkillTimelineDurationSeconds() => Owner.GetSkillActionDurationSeconds();

        private float AdvanceCastSkillTimeline(global::Units.Actions.ActionTickContext context)
        {
            return castSkillTimeline.Advance(
                context.DeltaTime,
                context.TimelineSpeed,
                GetCastSkillTimelineDurationSeconds());
        }

        private bool HasCompletedCastSkillTimeline(float timelineProgress)
        {
            return timelineProgress >= 1f;
        }

        protected override void OnTick(global::Units.Actions.ActionTickContext context)
        {
            float timelineProgress = AdvanceCastSkillTimeline(context);

            if (!HasCompletedCastSkillTimeline(timelineProgress))
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
        }
    }
}
