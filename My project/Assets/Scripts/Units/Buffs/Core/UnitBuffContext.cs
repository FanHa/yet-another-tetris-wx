using System;
using System.Collections.Generic;
using Controller;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// Buff 专用上下文适配器。
    /// 负责把 Buff 需要的读写能力映射到宿主 Unit。
    /// </summary>
    public sealed class UnitBuffContext : IBuffContext
    {
        private readonly Unit unit;

        public UnitBuffContext(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public Unit SelfUnit => unit;
        public bool IsActive => unit.IsActive;
        public Unit.Faction faction => unit.faction;
        public Transform transform => unit.transform;
        public Attributes Attributes => unit.Attributes;
        public UnitManager UnitManager => unit.UnitManager;

        public IReadOnlyList<Buff> GetActiveBuffsReadOnly()
        {
            return unit.GetActiveBuffsReadOnly();
        }

        public void AddBuff(Buff buff)
        {
            unit.AddBuff(buff);
        }

        public void RemoveBuff(Buff buff)
        {
            unit.RemoveBuff(buff);
        }

        public void AddBuffTo(Unit target, Buff buff)
        {
            target.AddBuff(buff);
        }

        public void RemoveBuffFrom(Unit target, Buff buff)
        {
            target.RemoveBuff(buff);
        }

        public void DealDamageTo(Unit target, Damages.Damage damage)
        {
            target.TakeDamage(damage);
        }

        public void AddSkillEnergy(float energy)
        {
            unit.AddSkillEnergy(energy);
        }

        public Movement.MovementResult MoveBy(Vector3 delta)
        {
            return unit.MoveBy(delta);
        }

        public void TakeDamage(Damages.Damage damage)
        {
            unit.TakeDamage(damage);
        }

        public void EnterStun()
        {
            unit.EnterStun();
        }

        public void ExitStun()
        {
            unit.ExitStun();
        }
    }
}
