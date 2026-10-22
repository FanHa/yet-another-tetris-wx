using Model.Buffs;

namespace Units.Buffs
{
    /// <summary>
    /// Chilled：多重减速Debuff
    /// </summary>
    public class Chilled : Buff
    {
        private readonly ChilledBuffDefinition chilledDefinition;
        private readonly float chilledDuration;
        public int MoveSlowPercent { get; }
        public int AttackSlowPercent { get; }
        public int ActionSlowPercent { get; }

        public Chilled(ChilledBuffDefinition definition, BuffDuration duration, int moveSlowPercent, int attackSlowPercent, int actionSlowPercent, BuffSource source)
            : base(definition, duration, source.SourceUnit, source.SourceSkill)
        {
            chilledDefinition = definition;
            chilledDuration = duration.Seconds;
            MoveSlowPercent = moveSlowPercent;
            AttackSlowPercent = attackSlowPercent;
            ActionSlowPercent = actionSlowPercent;
        }

        public override string Name() => chilledDefinition.DisplayName;
        public override string Description() =>
            chilledDefinition.DescribeEffect(chilledDuration, MoveSlowPercent, AttackSlowPercent, ActionSlowPercent);


        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -AttackSlowPercent);
            context.Attributes.MoveSpeed.AddPercentageModifier(this, -MoveSlowPercent);
            context.Attributes.ActionSpeed.AddPercentageModifier(this, -ActionSlowPercent);
        }
        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            context.Attributes.MoveSpeed.RemovePercentageModifier(this);
            context.Attributes.ActionSpeed.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}