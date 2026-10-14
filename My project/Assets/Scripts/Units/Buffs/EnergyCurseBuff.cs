namespace Units.Buffs
{
    public readonly struct EnergyCurseBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public float EnergyRegenReductionPercent { get; }
        public BuffSource Source { get; }
        public string StackId { get; }

        public EnergyCurseBuffArgs(float duration, float energyRegenReductionPercent, BuffSource source)
            : this(duration, energyRegenReductionPercent, source, string.Empty)
        {
        }

        public EnergyCurseBuffArgs(float duration, float energyRegenReductionPercent, BuffSource source, string stackId)
        {
            Duration = duration;
            EnergyRegenReductionPercent = energyRegenReductionPercent;
            Source = source;
            StackId = stackId;
        }
    }

    public class EnergyCurseBuff : Buff
    {
        public float EnergyRegenReductionPercent { get; }
        private readonly string stackId;

        public EnergyCurseBuff(Model.Buffs.BuffDefinition definition, EnergyCurseBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            EnergyRegenReductionPercent = args.EnergyRegenReductionPercent;
            stackId = args.StackId;
        }

        public override string Name() => Definition.DisplayName;
        public override string GetKey() => $"{base.GetKey()}:{stackId}";
        public override string Description() => $"{Definition.Description}\n能量回复速度降低 {EnergyRegenReductionPercent}%";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.EnergyPerSecond.AddPercentageModifier(this, -EnergyRegenReductionPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}
