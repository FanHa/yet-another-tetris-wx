using Units.Skills;
using Model.Buffs;

namespace Units.Buffs
{
    public readonly struct IceShieldBuffArgs : IBuffCreationArgs
    {
        public ChilledBuffDefinition ChilledDefinition { get; }
        public float BuffDuration { get; }
        public int ResonantCellCount { get; }
        public BuffSource Source { get; }

        public IceShieldBuffArgs(ChilledBuffDefinition chilledDefinition, float buffDuration, int resonantCellCount, BuffSource source)
        {
            ChilledDefinition = chilledDefinition;
            BuffDuration = buffDuration;
            ResonantCellCount = resonantCellCount;
            Source = source;
        }
    }

    /// <summary>
    /// 冰霜反击护盾：被攻击时对攻击者施加Chilled（多重减速）Debuff
    /// </summary>
    public class IceShield : Buff, ITakeHitTrigger
    {
        private readonly ChilledBuffDefinition chilledDefinition;
        private readonly float chilledDuration;
        private readonly int chilledMoveSlowPercent;
        private readonly int chilledAttackSlowPercent;
        private readonly int chilledActionSlowPercent;

        public IceShield(BuffDefinition definition, IceShieldBuffArgs args)
            : base(definition, args.BuffDuration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            chilledDefinition = args.ChilledDefinition;
            var stats = chilledDefinition.CalculateStats(args.ResonantCellCount);
            chilledDuration = stats.Duration.Final;
            chilledMoveSlowPercent = (int)stats.MoveSlowPercent.Final;
            chilledAttackSlowPercent = (int)stats.AttackSlowPercent.Final;
            chilledActionSlowPercent = (int)stats.ActionSlowPercent.Final;
        }

        public override string Name() => "冰霜护盾";
        public override string Description() =>
            $"被攻击时使攻击者获得Chilled：{chilledDefinition.DescribeEffect(chilledDuration, chilledMoveSlowPercent, chilledAttackSlowPercent, chilledActionSlowPercent)}";

        public void OnTakeHit(IBuffContext context, Unit self, Unit attacker, ref Damages.Damage damage)
        {
            if (attacker == null)
                return;
            var chilled = new Chilled(
                chilledDefinition,
                chilledDuration,
                chilledMoveSlowPercent,
                chilledAttackSlowPercent,
                chilledActionSlowPercent,
                new BuffSource(self, sourceSkill));
            context.AddBuffTo(attacker, chilled);
        }

    }
}