using Units.Skills;

namespace Units.Buffs
{
    public readonly struct ChilledBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public int MoveSlowPercent { get; }
        public int AttackSlowPercent { get; }
        public int ActionSlowPercent { get; }
        public int EnergyRegenSlowPercent { get; }
        public BuffSource Source { get; }

        public ChilledBuffArgs(float duration, int moveSlowPercent, int attackSlowPercent, int actionSlowPercent, int energyRegenSlowPercent, BuffSource source)
        {
            Duration = duration;
            MoveSlowPercent = moveSlowPercent;
            AttackSlowPercent = attackSlowPercent;
            ActionSlowPercent = actionSlowPercent;
            EnergyRegenSlowPercent = energyRegenSlowPercent;
            Source = source;
        }
    }

    /// <summary>
    /// Chilled：多重减速Debuff
    /// </summary>
    public class Chilled : Buff
    {
        public int MoveSlowPercent { get; }
        public int AttackSlowPercent { get; }
        public int ActionSlowPercent { get; }
        public int EnergyRegenSlowPercent { get; }

        public Chilled(Model.Buffs.BuffDefinition definition, ChilledBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            MoveSlowPercent = args.MoveSlowPercent;
            AttackSlowPercent = args.AttackSlowPercent;
            ActionSlowPercent = args.ActionSlowPercent;
            EnergyRegenSlowPercent = args.EnergyRegenSlowPercent;
        }

        public override string Name() => "Chilled";
        public override string Description() =>
            $"移速-{MoveSlowPercent}%，攻速-{AttackSlowPercent}%，动作速率-{ActionSlowPercent}%，能量回复-{EnergyRegenSlowPercent}%";


        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -AttackSlowPercent);
            context.Attributes.MoveSpeed.AddPercentageModifier(this, -MoveSlowPercent);
            context.Attributes.ActionSpeed.AddPercentageModifier(this, -ActionSlowPercent);
            context.Attributes.EnergyPerSecond.AddPercentageModifier(this, -EnergyRegenSlowPercent);
        }
        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            context.Attributes.MoveSpeed.RemovePercentageModifier(this);
            context.Attributes.ActionSpeed.RemovePercentageModifier(this);
            context.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}