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
    /// 移动类 Action 依赖的最小能力端口。
    /// </summary>
    public interface IMoveActionContext
    {
        float AttackRange { get; }
        float BodyRadius { get; }

        Unit.MoveBehaviorMode CurrentMoveBehaviorMode { get; }

        bool TryGetClosestEnemy(out Unit target);
        bool TryGetClosestAlly(out Unit target);

        Movement.MovementResult ApplyMovement(Movement.MovementRequest request);
    }

    /// <summary>
    /// 攻击类 Action 依赖的最小能力端口。
    /// </summary>
    public interface IAttackActionContext
    {
        float ActionSpeed { get; }
        Vector3 Position { get; }

        bool CanAttackNow();
        bool TryGetClosestEnemy(out Unit target);
        float GetEffectiveAttackRangeTo(Unit target);

        float GetAttackActionDurationSeconds();

        void ExecuteAttackProjectile(Unit target);
        void MarkAttackExecuted();

        void SuspendAutoMovement();
        void ResumeAutoMovement();
    }

    /// <summary>
    /// 技能类 Action 依赖的最小能力端口。
    /// </summary>
    public interface ISkillActionContext
    {
        float ActionSpeed { get; }
        bool HasReadySkill { get; }

        float GetSkillActionDurationSeconds();

        void ExecutePendingSkill();

        void SuspendAutoMovement();
        void ResumeAutoMovement();
    }

    /// <summary>
    /// 状态类 Action 依赖的最小能力端口。
    /// </summary>
    public interface IStatusActionContext
    {
        bool IsStunned { get; }
        bool IsSkillMotionActive { get; }

        void SuspendAutoMovement();
        void ResumeAutoMovement();
        void PrepareForSkillMotion();
    }
}
