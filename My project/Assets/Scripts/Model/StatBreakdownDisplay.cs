namespace Model
{
    public sealed class StatBreakdownDisplay
    {
        public StatLabel Label { get; }
        public AdditiveValueBreakdown Value { get; }
        public float FinalValue => Value.FinalValue;

        public StatBreakdownDisplay(StatLabel label, float baseValue, float addedAmount = 0, float reducedAmount = 0)
        {
            Label = label;
            Value = new AdditiveValueBreakdown(baseValue, addedAmount, reducedAmount);
        }

        public override string ToString() => StatBreakdownFormatter.Format(Label, Value);
    }
}