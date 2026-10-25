using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// 冰霜反击护盾：被攻击时对攻击者施加Chilled（多重减速）Debuff
    /// </summary>
    public class IceShield : Buff, ITakeHitTrigger
    {
        private readonly IceShieldBuffDefinition iceShieldDefinition;
        private readonly int resonantCellCount;
        private readonly ChilledBuffDefinition chilledDefinition;
        private readonly float chilledDuration;
        private readonly int chilledMoveSlowPercent;
        private readonly int chilledAttackSlowPercent;
        private readonly int chilledActionSlowPercent;

        public IceShield(IceShieldBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            iceShieldDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            chilledDefinition = definition.ChilledDefinition;
            var stats = chilledDefinition.CalculateStats(resonantCellCount);
            chilledDuration = stats.Duration.FinalValue;
            chilledMoveSlowPercent = (int)stats.MoveSlowPercent.FinalValue;
            chilledAttackSlowPercent = (int)stats.AttackSlowPercent.FinalValue;
            chilledActionSlowPercent = (int)stats.ActionSlowPercent.FinalValue;
        }

        public override string Name() => iceShieldDefinition.DisplayName;
        public override string DescribeAppliedEffect() =>
            iceShieldDefinition.DescribeEffect(resonantCellCount);

        public void OnTakeHit(IBuffContext context, Unit self, Unit attacker, ref Damages.Damage damage)
        {
            if (attacker == null)
                return;
            var chilled = new Chilled(
                chilledDefinition,
                BuffDuration.ForSeconds(chilledDuration),
                chilledMoveSlowPercent,
                chilledAttackSlowPercent,
                chilledActionSlowPercent,
                new BuffSource(self, sourceSkill));
            context.AddBuffTo(attacker, chilled);
        }

    }
}