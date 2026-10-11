namespace Units.Buffs
{
    public readonly struct CurseBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public float AttackReductionPercent { get; }
        public float EnergyRegenReductionPercent { get; }
        public BuffSource Source { get; }
        public string StackId { get; }

        public CurseBuffArgs(float duration, float attackReductionPercent, float energyRegenReductionPercent, BuffSource source)
            : this(duration, attackReductionPercent, energyRegenReductionPercent, source, string.Empty)
        {
        }

        public CurseBuffArgs(float duration, float attackReductionPercent, float energyRegenReductionPercent, BuffSource source, string stackId)
        {
            Duration = duration;
            AttackReductionPercent = attackReductionPercent;
            EnergyRegenReductionPercent = energyRegenReductionPercent;
            Source = source;
            StackId = stackId;
        }
    }

    public class CurseBuff : Buff
    {
        public float AttackReductionPercent { get; }
        public float EnergyRegenReductionPercent { get; }
        private readonly string stackId;

        public CurseBuff(Model.Buffs.BuffDefinition definition, CurseBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            AttackReductionPercent = args.AttackReductionPercent;
            EnergyRegenReductionPercent = args.EnergyRegenReductionPercent;
            stackId = args.StackId;
        }

        public override string Name() => Definition.DisplayName;
        public override string GetKey() => $"{base.GetKey()}:{stackId}";
        public override string Description() =>
            $"{Definition.Description}\nAttack power -{AttackReductionPercent}%, energy regeneration -{EnergyRegenReductionPercent}%";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttackPower.AddPercentageModifier(this, -AttackReductionPercent);
            context.Attributes.EnergyPerSecond.AddPercentageModifier(this, -EnergyRegenReductionPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.AttackPower.RemovePercentageModifier(this);
            context.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}