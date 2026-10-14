namespace Units.Buffs
{
    public readonly struct PowerCurseBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public float AttackReductionPercent { get; }
        public BuffSource Source { get; }
        public string StackId { get; }

        public PowerCurseBuffArgs(float duration, float attackReductionPercent, BuffSource source)
            : this(duration, attackReductionPercent, source, string.Empty)
        {
        }

        public PowerCurseBuffArgs(float duration, float attackReductionPercent, BuffSource source, string stackId)
        {
            Duration = duration;
            AttackReductionPercent = attackReductionPercent;
            Source = source;
            StackId = stackId;
        }
    }

    public class PowerCurseBuff : Buff
    {
        public float AttackReductionPercent { get; }
        private readonly string stackId;

        public PowerCurseBuff(Model.Buffs.BuffDefinition definition, PowerCurseBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            AttackReductionPercent = args.AttackReductionPercent;
            stackId = args.StackId;
        }

        public override string Name() => Definition.DisplayName;
        public override string GetKey() => $"{base.GetKey()}:{stackId}";
        public override string Description() => $"{Definition.Description}\n攻击力降低 {AttackReductionPercent}%";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttackPower.AddPercentageModifier(this, -AttackReductionPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.AttackPower.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}