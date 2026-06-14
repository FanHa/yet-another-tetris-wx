using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// WindKnockbackBuff：普通攻击命中时，将目标沿攻击方向击退一小段距离
    /// </summary>
    public class WindKnockbackBuff : Buff, IAttackHitTrigger
    {
        private readonly float knockbackDistance;
        private readonly float maxKnockbackDistance;

        public WindKnockbackBuff(
            float knockbackDistance,
            float maxKnockbackDistance,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(-1f, sourceUnit, sourceSkill)
        {
            this.knockbackDistance = knockbackDistance;
            this.maxKnockbackDistance = maxKnockbackDistance;
        }

        public override string Name() => "轻风击退";
        public override string Description() =>
            $"普攻命中时将目标击退{knockbackDistance:F2}（最多{maxKnockbackDistance:F2}）";

        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            if (target == null || !target.IsActive) return;

            // 计算攻击者到目标的水平方向（二维平面）
            Vector2 dir = ((Vector2)target.transform.position - (Vector2)attacker.transform.position).normalized;

            // 最终击退距离不超过配置上限
            float dist = Mathf.Min(knockbackDistance, maxKnockbackDistance);

            Vector3 delta = new Vector3(dir.x * dist, dir.y * dist, 0f);
            // 先清理导航路径，避免 NavMeshAgent 立即沿旧路径把目标拉回去。
            target.Movement.ClearNavigationPath();
            // 击退是一次性位移，使用 MoveStraightByDelta，避免被 softDisplacementMaxPerFrame 截断到 0.2。
            target.Movement.MoveStraightByDelta(delta);
        }
    }
}
