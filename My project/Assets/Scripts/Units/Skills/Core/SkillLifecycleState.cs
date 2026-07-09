namespace Units.Skills
{
    // Unified lifecycle milestones for active skill flow.
    public enum SkillLifecycleState
    {
        None,

        // Energy reached threshold and skill can enter candidate queue.
        ChargeReady,

        // Preconditions are satisfied at cast time (target/range/state).
        CastReady,

        // Cast attempt has started.
        CastStarted,

        // Skill effect has been committed successfully.
        CastCommitted,

        // Cast attempt ended without committing effect.
        CastFailed
    }
}
