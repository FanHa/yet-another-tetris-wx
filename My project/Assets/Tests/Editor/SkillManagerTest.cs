using NUnit.Framework;
using System.Collections.Generic;
using Units;
using Units.Skills;
using UnityEngine;

public class SkillManagerTests
{
    private Manager skillManager;
    private MockSkill mockSkill;
    private Unit mockUnit;

    [SetUp]
    public void Setup()
    {
        // 初始化 SkillManager 和 Mock 数据
        skillManager = new Manager();
        mockSkill = new MockSkill();
        mockUnit = new GameObject().AddComponent<Unit>();
    }

    [Test]
    public void AddSkill_ShouldAddSkillToList()
    {
        // Act
        skillManager.AddSkill(mockSkill);

        // Assert
        Assert.AreEqual(1, skillManager.SkillsCount, "SkillManager should contain 1 skill after adding.");
    }

    [Test]
    public void Initialize_ShouldCallInitOnAllSkills()
    {
        // Arrange
        skillManager.AddSkill(mockSkill);

        // Act
        skillManager.Initialize(mockUnit);

        // Assert
        Assert.IsTrue(mockSkill.IsInitialized, "Skill should be initialized after SkillManager.Initialize is called.");
    }

    [Test]
    public void CastSkills_ShouldExecuteReadySkills()
    {
        // Arrange
        skillManager.AddSkill(mockSkill);
        skillManager.Initialize(mockUnit);

        // Act
        skillManager.CastSkill();

        // Assert
        Assert.IsTrue(mockSkill.IsExecuted, "Skill should be executed if it is ready.");
    }

    [Test]
    public void AddSkill_ShouldIgnoreDuplicateSkills()
    {
        // Arrange
        skillManager.AddSkill(mockSkill);

        // Act
        skillManager.AddSkill(mockSkill); // 再次添加相同技能

        // Assert
        Assert.AreEqual(1, skillManager.SkillsCount, "SkillManager should not allow duplicate skills.");
    }

    [Test]
    public void CastSkills_ShouldDoNothing_WhenNoSkillsAreAdded()
    {
        // Act
        skillManager.CastSkill();

        // Assert
        // 如果没有技能，CastSkills 不应该抛出异常或执行任何操作
        Assert.Pass("CastSkill executed without errors when no skills were added.");
    }

    [Test]
    public void CastSkills_ShouldNotExecuteSkill_WhenSkillIsNotReady()
    {
        // Arrange
        var skillWithCooldown = new MockSkillWithCooldown();
        skillManager.AddSkill(skillWithCooldown);
        skillManager.Initialize(mockUnit);

        // Act
        skillManager.CastSkill(); // 第一次释放技能
        skillManager.CastSkill(); // 再次尝试释放技能

        // Assert
        Assert.AreEqual(1, skillWithCooldown.ExecutionCount, "Skill should not execute again if it is not ready.");
    }

    [Test]
    public void CooldownRevisePercentage_ShouldAffectSkillCooldown()
    {
        // Arrange
        var skillWithCooldown = new MockSkillWithCooldown();
        skillWithCooldown.OriginalCooldown = 3f;
        skillManager.AddSkill(skillWithCooldown);
        skillManager.CooldownRevisePercentage = 50f; // 冷却时间减少 50%
        skillManager.Initialize(mockUnit);

        // Act
        skillManager.CastSkill(); // 第一次释放技能
        skillWithCooldown.AdvanceCooldown(1f); // 模拟冷却时间推进 1 秒
        skillManager.CastSkill(); // 再次尝试释放技能

        // Assert
        Assert.AreEqual(1, skillWithCooldown.ExecutionCount, "Skill should not execute again if cooldown is not fully reduced.");
        
        // 模拟冷却时间推进足够时间
        skillWithCooldown.AdvanceCooldown(skillWithCooldown.OriginalCooldown / 2f);
        skillManager.CastSkill(); // 再次尝试释放技能
        Assert.AreEqual(2, skillWithCooldown.ExecutionCount, "Skill should execute again after cooldown is reduced by CooldownRevisePercentage.");
    }

}