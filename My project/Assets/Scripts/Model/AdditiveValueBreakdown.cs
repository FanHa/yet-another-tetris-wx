namespace Model
{
    public class AdditiveValueBreakdown
    {
        public float BaseValue { get; set; }
        public float AddedAmount { get; set; }
        public float ReducedAmount { get; set; }

        public float FinalValue => BaseValue + AddedAmount - ReducedAmount;

        public AdditiveValueBreakdown(float baseValue, float addedAmount = 0, float reducedAmount = 0)
        {
            BaseValue = baseValue;
            AddedAmount = addedAmount;
            ReducedAmount = reducedAmount;
        }
    }
}