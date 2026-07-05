namespace Units.Skills
{
    public readonly struct SkillCastResult
    {
        public bool Succeeded { get; }
        public Skill Skill { get; }
        public SkillCastFailureReason FailureReason { get; }

        public SkillCastResult(bool succeeded, Skill skill, SkillCastFailureReason failureReason)
        {
            Succeeded = succeeded;
            Skill = skill;
            FailureReason = failureReason;
        }

        public static SkillCastResult Success(Skill skill)
        {
            return new SkillCastResult(true, skill, SkillCastFailureReason.None);
        }

        public static SkillCastResult Failure(Skill skill, SkillCastFailureReason failureReason)
        {
            return new SkillCastResult(false, skill, failureReason);
        }
    }
}
