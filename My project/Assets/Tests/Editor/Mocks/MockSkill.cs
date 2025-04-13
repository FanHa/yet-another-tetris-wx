using Units;
using Units.Skills;
using UnityEngine;

public class MockSkill : Skill
{
    public bool IsInitialized { get; private set; }
    public bool IsExecuted { get; private set; }
    public Unit BoundUnit { get; private set; }


    public override void Init()
    {
        IsInitialized = true;
    }

    public override bool IsReady()
    {
        return true; // Always ready for testing
    }

    public override void Execute(Unit unit)
    {
        IsExecuted = true;
        BoundUnit = unit; // 记录绑定的 Unit
    }

    public override string Name()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExecuteCore(Unit caster)
    {
        throw new System.NotImplementedException();
    }

    public override string Description()
    {
        throw new System.NotImplementedException();
    }
}

public class MockSkillWithCooldown : Skill
{
    public int ExecutionCount { get; private set; }
    private float lastExecutionTime;

    public override void Init()
    {
        lastExecutionTime = -10f; // 初始化为冷却完成状态
    }

    public override bool IsReady()
    {
        return Time.time >= lastExecutionTime + 5f; // 冷却时间为 5 秒
    }

    public override void Execute(Unit unit)
    {
        lastExecutionTime = Time.time;
        ExecutionCount++;
    }

    public override string Name()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExecuteCore(Unit caster)
    {
        throw new System.NotImplementedException();
    }

    public override string Description()
    {
        throw new System.NotImplementedException();
    }
}