using System.Collections.Generic;
using UnityEngine;
using Controller;

namespace Units.Buffs
{
    /// <summary>
    /// Buff 运行时与宿主交互的唯一边界。
    /// 这里只放 Buff 回调真正会用到的宿主状态和能力，不承载 Unit 的完整职责。
    /// 状态读取和宿主操作都通过这份上下文完成。
    /// </summary>
    public interface IBuffContext
    {
        // 宿主状态读取
        Units.Unit SelfUnit { get; }
        bool IsActive { get; }
        Units.Unit.Faction faction { get; }
        Transform transform { get; }
        Units.Attributes Attributes { get; }
        UnitManager UnitManager { get; }

        // Buff 生命周期与查询
        IReadOnlyList<Buff> GetActiveBuffsReadOnly();

        // Buff 可驱动的宿主能力
        void AddBuff(Buff buff);
        void RemoveBuff(Buff buff);
        void AddBuffTo(Units.Unit target, Buff buff);
        void RemoveBuffFrom(Units.Unit target, Buff buff);
        void DealDamageTo(Units.Unit target, Units.Damages.Damage damage);
        void AddSkillEnergy(float energy);
        Units.Movement.MovementResult MoveBy(Vector3 delta);
        void TakeDamage(Units.Damages.Damage damage);
        void EnterStun();
        void ExitStun();
    }

}
