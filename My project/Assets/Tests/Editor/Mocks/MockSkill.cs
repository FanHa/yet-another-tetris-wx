using Units;
using Units.Skills;
using UnityEngine;

public class MockSkill : ActiveSkill
{
    public bool IsInitialized { get; private set; }
    public bool IsExecuted { get; private set; }
    public Unit BoundUnit { get; private set; }


    // public override void Init(float baseCoolDownPercentage)
    // {
    //     IsInitialized = true;
    // }

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

    protected override bool ExecuteCore(Unit caster)
    {
        throw new System.NotImplementedException();
    }

    public override string Description()
    {
        throw new System.NotImplementedException();
    }
}

public class MockSkillWithCooldown : ActiveSkill
{
    public int ExecutionCount { get; private set; } = 0;
    public float OriginalCooldown  = 2f; // 原始冷却时间
    private float remainingCooldown = 0f;

    // public override void Init(float cooldownRevisePercentage)
    // {
    //     OriginalCooldown *= cooldownRevisePercentage / 100f;
    //     remainingCooldown = 0f;
    // }

    public override bool IsReady()
    {
        return remainingCooldown <= 0f;
    }

    public override void Execute(Unit owner)
    {
        ExecutionCount++;
        remainingCooldown = OriginalCooldown;
    }

    public void AdvanceCooldown(float time)
    {
        remainingCooldown -= time;
    }

    public override string Name()
    {
        throw new System.NotImplementedException();
    }

    protected override bool ExecuteCore(Unit caster)
    {
        throw new System.NotImplementedException();
    }

    public override string Description()
    {
        throw new System.NotImplementedException();
    }
}