using System;
using System.Collections.Generic;
using Model;
using Model.Buffs;
using Units.Buffs;

internal class BuffTimingChecks
{
    private int failures;

    private static int Main()
    {
        var checks = new BuffTimingChecks();
        checks.Run("New buff keeps its full duration", checks.FullDuration);
        checks.Run("Fractional duration expires on time", checks.FractionalDuration);
        checks.Run("Refresh preserves the damage cadence", checks.RefreshDuration);
        checks.Run("Long frames catch up every damage tick", checks.LongFrame);
        checks.Run("Expired buff cannot receive a later tick", checks.ExpiryBeforeTick);
        checks.Run("Tick at the expiry boundary is retained", checks.ExpiryAtTick);
        checks.Run("Cleared buffs cannot tick from an old snapshot", checks.ClearDuringTick);
        checks.Run("Tick-created buff advances only after application", checks.AddDuringTick);
        checks.Run("Zero elapsed time only commits requests", checks.ZeroElapsedTime);
        checks.Run("DPS is independent of tick interval", checks.DamageAcrossTickIntervals);
        checks.Run("First partial tick uses actual buff age", checks.PartialFirstTick);
        checks.Run("Refresh preserves accrued damage", checks.RefreshAccruedDamage);
        checks.Run("Explicit removal settles the final fraction", checks.RemovalAccruedDamage);
        checks.Run("Cleanup discards unsettled damage", checks.ClearAccruedDamage);
        checks.Run("Lethal settlement may clear the handler", checks.ClearDuringSettlement);
        checks.Run("Removal cancels a queued refresh", checks.RemoveDuringPendingRefresh);
        checks.Run("FlameInject descriptions use numeric snapshots", checks.FlameInjectDescriptions);
        checks.Run("FlameInject attacks refresh the original burn snapshot", checks.FlameInjectAttacks);
        Console.WriteLine($"{18 - checks.failures}/18 checks passed.");
        return checks.failures == 0 ? 0 : 1;
    }

    private void FullDuration()
    {
        var handler = CreateHandler();
        handler.ApplyBuff(new TestBuff("keeper", BuffDuration.Permanent));
        handler.Tick(0.75f);
        var buff = new TestBuff("timed", BuffDuration.ForSeconds(5f));
        handler.ApplyBuff(buff);
        handler.Tick(0.25f);
        Equal(4.75f, buff.TimeLeft);
        handler.Tick(4.5f);
        Equal(0.25f, buff.TimeLeft);
        Active(handler, buff, true);
        handler.Tick(0.25f);
        Active(handler, buff, false);
    }

    private void FractionalDuration()
    {
        var handler = CreateHandler();
        var buff = new TestBuff("fractional", BuffDuration.ForSeconds(0.5f));
        handler.ApplyBuff(buff);
        handler.Tick(0.25f);
        Equal(0.25f, buff.TimeLeft);
        Active(handler, buff, true);
        handler.Tick(0.25f);
        Active(handler, buff, false);
    }

    private void RefreshDuration()
    {
        var handler = CreateHandler();
        var buff = new TickBuff("burn", BuffDuration.ForSeconds(5f));
        handler.ApplyBuff(buff);
        handler.Tick(0.75f);
        handler.ApplyBuff(new TickBuff("burn", BuffDuration.ForSeconds(5f)));
        handler.CommitPendingChanges();
        Equal(5f, buff.TimeLeft);
        handler.Tick(0.25f);
        Equal(1f, buff.TickCount);
        Equal(4.75f, buff.TimeLeft);
        Equal(1f, handler.GetActiveBuffsReadOnly().Count);
        handler.Tick(4.5f);
        Equal(5f, buff.TickCount);
        Equal(0.25f, buff.TimeLeft);
        handler.Tick(0.25f);
        Active(handler, buff, false);
    }

    private void LongFrame()
    {
        var handler = CreateHandler();
        var buff = new TickBuff("permanent", BuffDuration.Permanent);
        handler.ApplyBuff(buff);
        handler.Tick(4.25f);
        Equal(4f, buff.TickCount);
        handler.Tick(0.75f);
        Equal(5f, buff.TickCount);
    }

    private void ExpiryBeforeTick()
    {
        var handler = CreateHandler();
        var buff = new TickBuff("short", BuffDuration.ForSeconds(0.5f));
        handler.ApplyBuff(buff);
        handler.Tick(1.5f);
        Equal(1f, buff.TickCount);
        Equal(0.5f, buff.ElapsedSeconds);
        Active(handler, buff, false);
    }

    private void ExpiryAtTick()
    {
        var handler = CreateHandler();
        var buff = new TickBuff("boundary", BuffDuration.ForSeconds(1f));
        handler.ApplyBuff(buff);
        handler.Tick(1f);
        Equal(1f, buff.TickCount);
        Active(handler, buff, false);
    }

    private void ClearDuringTick()
    {
        var handler = CreateHandler();
        var other = new TickBuff("other", BuffDuration.Permanent);
        var clearing = new TickBuff("clearing", BuffDuration.Permanent, handler.RemoveAllActiveBuffsImmediately);
        handler.ApplyBuff(other);
        handler.ApplyBuff(clearing);
        handler.Tick(2f);
        Equal(1f, clearing.TickCount);
        Equal(0f, other.TickCount);
        Equal(0f, handler.GetActiveBuffsReadOnly().Count);
    }

    private void AddDuringTick()
    {
        var handler = CreateHandler();
        var child = new TickBuff("child", BuffDuration.ForSeconds(1.25f));
        bool added = false;
        var spawner = new TickBuff("spawner", BuffDuration.Permanent, () =>
        {
            if (added)
                return;
            added = true;
            handler.ApplyBuff(child);
        });
        handler.ApplyBuff(spawner);
        handler.Tick(2.5f);
        Equal(2f, child.TickCount);
        Equal(1.25f, child.ElapsedSeconds);
        Active(handler, child, false);
    }

    private void ZeroElapsedTime()
    {
        var handler = CreateHandler();
        var buff = new TickBuff("zero", BuffDuration.ForSeconds(5f));
        handler.ApplyBuff(buff);
        handler.Tick(0f);
        Equal(5f, buff.TimeLeft);
        Equal(0f, buff.TickCount);
        Active(handler, buff, true);
    }

    private BuffHandler CreateHandler() => new BuffHandler(new TestContext());

    private Burn CreateBurn(float seconds) => new Burn(
        new BurnBuffDefinition(), 10f, BuffDuration.ForSeconds(seconds),
        new BuffSource(new Units.Unit(), new Units.Skills.Skill()));

    private void DamageAcrossTickIntervals()
    {
        foreach (float interval in new[] { 0.25f, 0.5f, 1f, 2f })
        {
            foreach (float frameTime in new[] { 0.125f, 2.5f })
            {
                var context = new TestContext();
                var handler = new BuffHandler(context, interval);
                var burn = CreateBurn(2.5f);
                handler.ApplyBuff(burn);
                for (float elapsed = 0f; elapsed < 2.5f; elapsed += frameTime)
                    handler.Tick(frameTime);
                Equal(25f, context.DamageTaken);
                Active(handler, burn, false);
            }
        }
    }

    private void PartialFirstTick()
    {
        var context = new TestContext();
        var handler = new BuffHandler(context);
        handler.ApplyBuff(new TestBuff("keeper", BuffDuration.Permanent));
        handler.Tick(0.75f);
        handler.ApplyBuff(CreateBurn(2.5f));
        handler.Tick(0.25f);
        Equal(2.5f, context.DamageTaken);
        handler.Tick(2.25f);
        Equal(25f, context.DamageTaken);
    }

    private void RefreshAccruedDamage()
    {
        var context = new TestContext();
        var handler = new BuffHandler(context);
        var burn = CreateBurn(1.25f);
        handler.ApplyBuff(burn);
        handler.Tick(0.75f);
        handler.ApplyBuff(CreateBurn(1.25f));
        handler.CommitPendingChanges();
        handler.Tick(0.25f);
        Equal(10f, context.DamageTaken);
        handler.Tick(1f);
        Equal(20f, context.DamageTaken);
        Active(handler, burn, false);
    }

    private void RemovalAccruedDamage()
    {
        var context = new TestContext();
        var handler = new BuffHandler(context);
        var burn = CreateBurn(5f);
        handler.ApplyBuff(burn);
        handler.Tick(0.25f);
        handler.RemoveBuff(burn);
        handler.CommitPendingChanges();
        Equal(2.5f, context.DamageTaken);
        Active(handler, burn, false);
        handler.Tick(2f);
        Equal(2.5f, context.DamageTaken);
    }

    private void ClearAccruedDamage()
    {
        var context = new TestContext();
        var handler = new BuffHandler(context);
        handler.ApplyBuff(CreateBurn(5f));
        handler.Tick(0.25f);
        handler.RemoveAllActiveBuffsImmediately();
        Equal(0f, context.DamageTaken);
        Equal(0f, handler.GetActiveBuffsReadOnly().Count);
    }

    private void ClearDuringSettlement()
    {
        var context = new TestContext();
        var handler = new BuffHandler(context);
        context.OnDamage = handler.RemoveAllActiveBuffsImmediately;
        var burn = CreateBurn(0.5f);
        handler.ApplyBuff(burn);
        handler.Tick(1f);
        Equal(5f, context.DamageTaken);
        Active(handler, burn, false);
    }

    private void RemoveDuringPendingRefresh()
    {
        foreach (bool removeBySameKey in new[] { false, true })
        {
            var context = new TestContext();
            var handler = new BuffHandler(context);
            var burn = CreateBurn(5f);
            handler.ApplyBuff(burn);
            handler.Tick(0.25f);
            var replacement = CreateBurn(5f);
            handler.ApplyBuff(replacement);
            handler.RemoveBuff(removeBySameKey ? replacement : burn);
            handler.CommitPendingChanges();
            Equal(2.5f, context.DamageTaken);
            Equal(0f, handler.GetActiveBuffsReadOnly().Count);
        }
    }

    private void FlameInjectDescriptions()
    {
        var burnDefinition = new BurnBuffDefinition
        {
            DpsStats = new AdditiveValueBreakdown(2f, 6f),
            DurationStats = new AdditiveValueBreakdown(5f)
        };
        var definition = new FlameInjectBuffDefinition { BurnDefinition = burnDefinition };
        var buff = new FlameInject(definition, 3, new BuffSource(new Units.Unit(), new Units.Skills.Skill()));
        Equal(3f, burnDefinition.LastResonanceCount);
        burnDefinition.DpsStats = new AdditiveValueBreakdown(999f);
        string preview = buff.DescribeEffect();
        Contains(preview, "<final>8</final>");
        Contains(preview, "<bonus>6</bonus>");
        Contains(preview, "<final>5</final>");
        string applied = buff.DescribeAppliedEffect();
        Contains(applied, "8");
        Contains(applied, "5");
        if (preview.Contains("Model.AdditiveValueBreakdown") || applied.Contains("<final>"))
            throw new InvalidOperationException("Descriptions contain a raw type name or unconverted preview tags.");
    }

    private void FlameInjectAttacks()
    {
        var burnDefinition = new BurnBuffDefinition
        {
            DpsStats = new AdditiveValueBreakdown(2f, 6f),
            DurationStats = new AdditiveValueBreakdown(5f)
        };
        var definition = new FlameInjectBuffDefinition { BurnDefinition = burnDefinition };
        var attacker = new Units.Unit();
        var skill = new Units.Skills.Skill();
        var buff = new FlameInject(definition, 3, new BuffSource(attacker, skill));
        burnDefinition.DpsStats = new AdditiveValueBreakdown(999f);
        var context = new TestContext();
        var damage = new Units.Damages.Damage(0f, Units.Damages.DamageType.Skill);
        buff.OnAttackHit(context, attacker, new Units.Unit(), ref damage);
        var burn = context.AppliedBuffs[0];
        if (!ReferenceEquals(burn.SourceUnit, attacker) || !ReferenceEquals(burn.SourceSkill, skill)
            || !ReferenceEquals(burn.Definition, burnDefinition))
            throw new InvalidOperationException("Burn lost its original source or definition.");
        var handler = new BuffHandler(context);
        handler.ApplyBuff(burn);
        handler.Tick(0.75f);
        buff.OnAttackHit(context, attacker, new Units.Unit(), ref damage);
        handler.ApplyBuff(context.AppliedBuffs[1]);
        handler.CommitPendingChanges();
        Equal(1f, handler.GetActiveBuffsReadOnly().Count);
        Equal(5f, burn.TimeLeft);
        handler.Tick(0.25f);
        Equal(8f, context.DamageTaken);
        Equal(4.75f, burn.TimeLeft);
        handler.Tick(4.75f);
        Equal(46f, context.DamageTaken);
        Active(handler, burn, false);
    }

    private void Contains(string value, string expected)
    {
        if (!value.Contains(expected))
            throw new InvalidOperationException($"Expected text '{expected}' in '{value}'.");
    }

    private void Run(string name, Action check)
    {
        try
        {
            check();
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception exception)
        {
            failures++;
            Console.WriteLine($"FAIL {name}: {exception.Message}");
        }
    }

    private void Equal(float expected, float actual)
    {
        if (Math.Abs(expected - actual) > 0.0001f)
            throw new InvalidOperationException($"Expected {expected}, got {actual}.");
    }

    private void Active(BuffHandler handler, Buff buff, bool expected)
    {
        bool actual = false;
        foreach (Buff active in handler.GetActiveBuffsReadOnly())
        {
            if (ReferenceEquals(active, buff))
                actual = true;
        }
        if (actual != expected)
            throw new InvalidOperationException($"Expected active={expected}, got {actual}.");
    }

    private class TestContext : IBuffContext
    {
        public Units.Unit SelfUnit { get; } = new Units.Unit();
        public float DamageTaken { get; private set; }
        public Action OnDamage { get; set; }
        public List<Buff> AppliedBuffs { get; } = new List<Buff>();

        public void AddBuffTo(Units.Unit target, Buff buff) => AppliedBuffs.Add(buff);

        public void TakeDamage(Units.Damages.Damage damage)
        {
            DamageTaken += damage.Value;
            OnDamage?.Invoke();
        }
    }

    private class TestBuff : Buff
    {
        private readonly string key;

        public TestBuff(string key, BuffDuration duration) : base(null, duration, null, null)
        {
            this.key = key;
        }

        public override string Name() => key;
        public override string DescribeAppliedEffect() => key;
    }

    private class TickBuff : TestBuff, ITick
    {
        private readonly Action onTick;
        public int TickCount { get; private set; }
        public float ElapsedSeconds { get; private set; }

        public TickBuff(string key, BuffDuration duration, Action onTick = null) : base(key, duration)
        {
            this.onTick = onTick;
        }

        public void OnTick(IBuffContext context, float elapsedSeconds)
        {
            TickCount++;
            ElapsedSeconds += elapsedSeconds;
            onTick?.Invoke();
        }
    }
}

namespace UnityEngine
{
    internal class SerializeField : Attribute { }
    internal class Debug
    {
        public static void LogWarning(string message) => Console.Error.WriteLine(message);
    }
}

namespace Model.Buffs
{
    public class BuffDefinition
    {
        public string DisplayName => "burn";
        public string Description => "effect";
        public BuffDuration BaseDuration => BuffDuration.Permanent;
    }
    public class BurnBuffDefinition : BuffDefinition
    {
        public AdditiveValueBreakdown DpsStats { get; set; } = new AdditiveValueBreakdown(10f);
        public AdditiveValueBreakdown DurationStats { get; set; } = new AdditiveValueBreakdown(5f);
        public int LastResonanceCount { get; private set; }

        public (AdditiveValueBreakdown Dps, AdditiveValueBreakdown Duration) CalculateStats(int resonantCellCount)
        {
            LastResonanceCount = resonantCellCount;
            return (DpsStats, DurationStats);
        }
    }
    public class FlameInjectBuffDefinition : BuffDefinition
    {
        public BurnBuffDefinition BurnDefinition { get; set; }
    }
}

namespace Units
{
    public class Unit { }
}

namespace Units.Skills
{
    public class Skill
    {
        public string Name() => "source";
    }
}

namespace Units.Damages
{
    public enum DamageType { Skill }
    public class Damage
    {
        public float Value { get; }
        public Damage(float value, DamageType type) { Value = value; }
        public void SetSourceUnit(Unit unit) { }
        public void SetTargetUnit(Unit unit) { }
        public void SetSourceLabel(string label) { }
    }
}

namespace Units.Buffs
{
    public interface IBuffContext
    {
        Unit SelfUnit { get; }
        void TakeDamage(Units.Damages.Damage damage);
        void AddBuffTo(Units.Unit target, Buff buff);
    }
}