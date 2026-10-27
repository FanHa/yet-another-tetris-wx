using System;
using Model.Buffs;
using UnityEngine;

namespace Units.Buffs
{
    public enum BuffLifecycleState
    {
        None,
        PendingAdd,
        Active,
        PendingRemove,
        Removed
    }

    public abstract class Buff
    {
        private readonly BuffDuration duration;
        public float TimeLeft { get; private set; }
        public bool IsPermanent => duration.IsPermanent;
        protected BuffDuration Duration => duration;
        public BuffDefinition Definition { get; }
        protected Unit sourceUnit { get; set; }
        public Unit SourceUnit => sourceUnit;
        protected IBuffContext context;
        public IBuffContext Context => context;
        protected Unit owner => context?.SelfUnit;
        protected Units.Skills.Skill sourceSkill { get; set; } // 施加此Buff的技能
        public Units.Skills.Skill SourceSkill => sourceSkill;
        public event Action Removed;

        protected Buff(BuffDefinition definition, BuffDuration duration, Unit sourceUnit, Units.Skills.Skill sourceSkill)
        {
            Definition = definition;
            this.duration = duration;
            this.sourceUnit = sourceUnit;
            this.sourceSkill = sourceSkill;
        }

        public abstract string Name();
        public abstract string DescribeAppliedEffect();
        public virtual Type BuffType => GetType();
        public virtual bool ReplaceOnReapply => false;

        protected string DescribeDuration() =>
            Duration.IsPermanent
                ? $"{Name()}持续时间: 永久"
                : $"{Name()}持续时间: {Duration.Seconds} 秒";

        public virtual void OnApply(IBuffContext context)
        {
            ResetTimeLeft();
            this.context = context;
        }
        public virtual void OnRemove()
        {
            Removed?.Invoke();
        }

        public virtual string GetKey()
        {
            // 使用Buff的类型和名称作为唯一标识
            return $"{BuffType.FullName}:{Name()}";
        }
        public void RefreshDuration() => ResetTimeLeft();

        public bool IsExpired()
        {
            return duration.IsTimed && TimeLeft <= 0f;
        }
        public virtual void UpdateTime(float interval)
        {
            if (duration.IsTimed)
                TimeLeft -= interval;
        }

        private void ResetTimeLeft()
        {
            TimeLeft = duration.IsTimed ? duration.Seconds : 0f;
        }
    }

    public interface IAttackHitTrigger
    {
        void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage);
    }
    public interface ITakeHitTrigger
    {
        void OnTakeHit(IBuffContext context, Unit self, Unit attacker, ref Damages.Damage damage);
    }
    public interface ITick
    {
        void OnTick(IBuffContext context, float elapsedSeconds);
    }

    public interface IAfterTakeDamageTrigger
    {
        void OnAfterTakeDamage(IBuffContext context, ref Damages.Damage damage);
    }

    public interface IAnyUnitSkillCastTrigger
    {
        void OnAnyUnitSkillCast(IBuffContext context, Units.Unit caster, Units.Skills.Skill skill);
    }
    
}
