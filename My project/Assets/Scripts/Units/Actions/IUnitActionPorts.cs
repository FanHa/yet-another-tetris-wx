using UnityEngine;

namespace Units.Actions
{
    /// <summary>
    /// Runner 在调度层读取的最小状态端口。
    /// </summary>
    public interface IUnitActionRunnerContext
    {
        bool IsStunned { get; }
        bool IsSkillMotionActive { get; }
    }

    /// <summary>
    /// Action 执行层依赖的最小能力端口。
    /// </summary>
    public interface IUnitActionContext
    {
        bool IsStunned { get; }
        bool IsSkillMotionActive { get; }
        bool HasReadySkill { get; }

        float ActionSpeed { get; }
        float AttackRange { get; }
        float BodyRadius { get; }

        Vector3 Position { get; }
        Unit.MoveBehaviorMode CurrentMoveBehaviorMode { get; }

        bool CanAttackNow();
        bool TryGetClosestEnemy(out Unit target);
        bool TryGetClosestAlly(out Unit target);
        float GetEffectiveAttackRangeTo(Unit target);

        float GetAttackActionDurationSeconds();
        float GetSkillActionDurationSeconds();

        void ExecuteAttackProjectile(Unit target);
        void MarkAttackExecuted();
        void ExecutePendingSkill();

        void PauseNavigation();
        void ResumeNavigation();
        void ClearNavigationPathForSkillMotion();

        Movement.MovementResult ApplyMovement(Movement.MovementRequest request);
    }
}
