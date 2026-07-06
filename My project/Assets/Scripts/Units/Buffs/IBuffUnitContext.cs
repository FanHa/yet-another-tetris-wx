using System.Collections.Generic;
using UnityEngine;
using Controller;

namespace Units.Buffs
{
    /// <summary>
    /// Buff 事件触发过程中，单位侧需要暴露的共用运行时能力集合。
    /// 所有 Buff 事件共用这一份上下文，事件差异通过方法参数表达。
    /// </summary>
    public interface IBuffEventContext
    {
        Units.Unit SelfUnit { get; }
        bool IsActive { get; }
        Units.Unit.Faction faction { get; }
        Transform transform { get; }
        Units.Attributes Attributes { get; }
        UnitManager UnitManager { get; }

        IReadOnlyList<Buff> GetActiveBuffsReadOnly();

        void AddBuff(Buff buff);
        void RemoveBuff(Buff buff);
        void AddSkillEnergy(float energy);
        Units.Movement.MovementResult MoveBy(Vector3 delta);
        void TakeDamage(Units.Damages.Damage damage);
    }

}
