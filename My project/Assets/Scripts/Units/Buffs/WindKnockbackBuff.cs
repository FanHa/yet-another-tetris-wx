using Model;
using Model.Buffs;
using Presentation;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// WindKnockbackBuff：普通攻击命中时，将目标沿攻击方向击退一小段距离
    /// </summary>
    public class WindKnockbackBuff : Buff, IAttackHitTrigger
    {
        private readonly WindKnockbackBuffDefinition windKnockbackDefinition;
        private readonly AdditiveValueBreakdown knockbackDistanceStats;
        private readonly AdditiveValueBreakdown maxKnockbackDistanceStats;
        private readonly float knockbackDistance;
        private readonly float maxKnockbackDistance;

        public WindKnockbackBuff(WindKnockbackBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            windKnockbackDefinition = definition;
            var stats = definition.CalculateStats(resonantCellCount);
            knockbackDistanceStats = stats.KnockbackDistance;
            maxKnockbackDistanceStats = stats.MaxKnockbackDistance;
            knockbackDistance = Mathf.Min(stats.KnockbackDistance.FinalValue, stats.MaxKnockbackDistance.FinalValue);
            maxKnockbackDistance = stats.MaxKnockbackDistance.FinalValue;
        }

        public override string Name() => windKnockbackDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"普攻命中时将目标击退{knockbackDistance:F2}（最多{maxKnockbackDistance:F2}）\n" +
            $"{StatBreakdownFormatter.Format(StatLabel.KnockbackDistance, knockbackDistanceStats)}\n" +
            $"{StatBreakdownFormatter.Format(StatLabel.MaxKnockbackDistance, maxKnockbackDistanceStats)}\n" +
            Duration.ToDisplayString("轻风击退持续时间");

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            if (target == null || !target.IsActive) return;

            // 计算攻击者到目标的水平方向（二维平面）
            Vector2 dir = ((Vector2)target.transform.position - (Vector2)attacker.transform.position).normalized;

            // 最终击退距离不超过配置上限
            float dist = Mathf.Min(knockbackDistance, maxKnockbackDistance);

            Vector3 delta = new Vector3(dir.x * dist, dir.y * dist, 0f);
            // 击退通过统一位移接口，会自动清理导航路径并执行位移。
            target.MoveBy(delta);
        }
    }
}
