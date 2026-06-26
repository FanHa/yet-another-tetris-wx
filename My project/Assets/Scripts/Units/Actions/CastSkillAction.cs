namespace Units.Actions
{
    public sealed class CastSkillAction : UnitAction
    {
        public override int Priority => 20;
        private bool hasExecuted;
        private float castSkillElapsedSeconds;

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
            castSkillElapsedSeconds = 0f;
        }

        private void AdvanceCastSkillTimeline(global::Units.Actions.ActionTickContext context)
        {
            castSkillElapsedSeconds += context.DeltaTime * UnityEngine.Mathf.Max(0f, context.TimelineSpeed);
        }

        private bool HasCompletedCastSkillTimeline()
        {
            float durationSeconds = UnityEngine.Mathf.Max(0.001f, Owner.GetSkillActionDurationSeconds());
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
