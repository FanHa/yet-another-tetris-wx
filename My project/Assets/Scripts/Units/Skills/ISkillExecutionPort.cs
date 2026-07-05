using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// SkillHandler 在执行技能效果时使用的最小端口。
    /// 只包含副作用能力，不包含目标查询或运行态判定。
    /// </summary>
    public interface ISkillExecutionPort
    {
        Coroutine StartCoroutine(System.Collections.IEnumerator routine);

        void SetMoveBehaviorMode(Unit.MoveBehaviorMode mode);
        void Teleport(Vector3 position);
        Units.Movement.MovementResult MoveBy(Vector3 delta);
        void AddBuff(Units.Buffs.Buff buff);
    }
}
