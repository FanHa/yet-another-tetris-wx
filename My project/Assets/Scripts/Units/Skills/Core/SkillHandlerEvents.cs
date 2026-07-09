namespace Units.Skills
{
    public readonly struct SkillQueuedEvent
    {
        public Unit Owner { get; }
        public ActiveSkill Skill { get; }
        public float CurrentEnergy { get; }
        public float RequiredEnergy { get; }

        public SkillQueuedEvent(Unit owner, ActiveSkill skill, float currentEnergy, float requiredEnergy)
        {
            Owner = owner;
            Skill = skill;
            CurrentEnergy = currentEnergy;
            RequiredEnergy = requiredEnergy;
        }
    }

    public readonly struct SkillCastStartedEvent
    {
        public Unit Owner { get; }
        public ActiveSkill Skill { get; }
        public SkillLifecycleState State => SkillLifecycleState.CastStarted;

        public SkillCastStartedEvent(Unit owner, ActiveSkill skill)
        {
            Owner = owner;
            Skill = skill;
        }
    }

    public readonly struct SkillCastSucceededEvent
    {
        public Unit Owner { get; }
        public Skill Skill { get; }
        public SkillLifecycleState State => SkillLifecycleState.CastCommitted;

        public SkillCastSucceededEvent(Unit owner, Skill skill)
        {
            Owner = owner;
            Skill = skill;
        }
    }

    public readonly struct SkillCastFailedEvent
    {
        public Unit Owner { get; }
        public Skill Skill { get; }
        public SkillCastFailureReason Reason { get; }
        public SkillLifecycleState State => SkillLifecycleState.CastFailed;

        public SkillCastFailedEvent(Unit owner, Skill skill, SkillCastFailureReason reason)
        {
            Owner = owner;
            Skill = skill;
            Reason = reason;
        }
    }
}
