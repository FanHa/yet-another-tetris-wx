using System;
using Model.Tetri.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public abstract class Buff
    {
        protected float duration; // -1为永久
        protected float timeLeft;
        protected Unit sourceUnit { get; set; }
        protected Units.Skills.Skill sourceSkill { get; set; } // 施加此Buff的技能

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
            timeLeft = duration;
            OnApplyExtra(unit); // 调用子类扩展
        }
        protected virtual void OnApplyExtra(Unit unit) { }
        public virtual void OnRemove(Unit unit) { }

        public virtual string GetKey()
        {
            // 使用Buff的类型和名称作为唯一标识
            return $"{BuffType.FullName}:{Name()}";
        }
        public void RefreshDuration() => timeLeft = duration;

        public bool IsExpired()
        {
            return duration >= 0f && timeLeft <= 0f;
        }
        public virtual void UpdateTime(float interval)
        {
            if (duration > 0)
                timeLeft -= interval;
        }
    }

    public interface IAttackHitTrigger {
        void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage);
    }
    public interface ITakeHitTrigger {
        void OnTakeHit(Unit self, Unit attacker, ref Damages.Damage damage);
    }
    public interface ITick {
        void OnTick(Unit unit);
    }
}
