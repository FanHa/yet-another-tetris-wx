using System;
using Model.Tetri.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public abstract class Buff
    {
        protected float duration; // -1为永久
        public float TimeLeft;
        protected Unit sourceUnit { get; set; }
        public Unit SourceUnit => sourceUnit;
        protected Unit owner;
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

        public virtual void OnApply(Unit unit)
        {
            TimeLeft = duration;
            owner = unit;
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
        void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage);
    }
    public interface ITakeHitTrigger
    {
        void OnTakeHit(Unit self, Unit attacker, ref Damages.Damage damage);
    }
    public interface ITick
    {
        void OnTick(Unit unit);
    }

    public interface IAfterTakeDamageTrigger
    {
        void OnAfterTakeDamage(ref Damages.Damage damage);
    }
    
}
