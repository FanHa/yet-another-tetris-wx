using System;
using Model.Tetri.Skills;
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
        protected float duration; // -1为永久
        public float TimeLeft;
        protected Unit sourceUnit { get; set; }
        public Unit SourceUnit => sourceUnit;
        protected IBuffContext context;
        public IBuffContext Context => context;
        protected Unit owner => context?.SelfUnit;
        protected Units.Skills.Skill sourceSkill { get; set; } // 施加此Buff的技能
        public Units.Skills.Skill SourceSkill => sourceSkill;
        public event Action Removed;

        public Buff(float duration, Unit sourceUnit, Units.Skills.Skill sourceSkill)
        {
            this.duration = duration;
            this.sourceUnit = sourceUnit;
            this.sourceSkill = sourceSkill;
        }

        public abstract string Name();
        public abstract string Description();
        public virtual Type BuffType => GetType();

        public virtual void OnApply(IBuffContext context)
        {
            TimeLeft = duration;
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
        public void RefreshDuration() => TimeLeft = duration;

        public bool IsExpired()
        {
            return duration >= 0f && TimeLeft <= 0f;
        }
        public virtual void UpdateTime(float interval)
        {
            if (duration > 0)
                TimeLeft -= interval;
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
        void OnTick(IBuffContext context);
    }

    public interface IAfterTakeDamageTrigger
    {
        void OnAfterTakeDamage(IBuffContext context, ref Damages.Damage damage);
    }

    public interface IGlobalSkillCastTrigger
    {
        void OnGlobalSkillCast(IBuffContext context, Units.Unit caster, Units.Skills.Skill skill);
    }
    
}
