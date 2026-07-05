namespace Units.Skills
{
    // Why a cast attempt did not commit any skill effect.
    public enum SkillCastFailureReason
    {
        None,

        // No pending/candidate skill is available for this cast window.
        NoPendingSkill,

        // Runtime preconditions are not satisfied (state/range/cooldown gate).
        PrerequisiteNotMet,

        // Target-dependent check failed (missing, dead, or invalid target).
        InvalidTarget,

        // Execution started but core logic failed to commit effect.
        ExecuteCoreFailed,

        // Cast flow was canceled by external state (stun/deactivate/interrupt).
        CanceledByState,

        // Defensive fallback for unexpected exceptions.
        Exception
    }
}
