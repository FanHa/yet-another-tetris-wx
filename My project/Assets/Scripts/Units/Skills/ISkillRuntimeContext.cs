namespace Units.Skills
{
    /// <summary>
    /// SkillHandler 在运行时依赖的最小上下文。
    /// 先继承现有的技能上下文能力，再补充运行态状态位，避免后续拆分重复搬运。
    /// </summary>
    public interface ISkillRuntimeContext : IUnitSkillContext
    {
        bool IsActive { get; }
        bool IsStunned { get; }
        bool IsSkillMotionActive { get; }
        float ActionSpeed { get; }
    }
}
