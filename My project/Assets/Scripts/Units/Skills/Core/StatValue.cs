namespace Units.Skills
{
    public class StatValue
    {
        public string Label { get; set; }
        public float Base { get; set; }
        public float Bonus { get; set; }
        public float Debuff { get; set; }

        public float Final => Base + Bonus - Debuff;

        public StatValue(string label, float baseValue, float bonus = 0, float debuff = 0)
        {
            Label = label;
            Base = baseValue;
            Bonus = bonus;
            Debuff = debuff;
        }

        public override string ToString()
        {
            // 都为0，直接显示
            if (Bonus == 0 && Debuff == 0)
                return $"{Label}: <final>{Final}</final>";

            // 只有Bonus
            if (Bonus != 0 && Debuff == 0)
                return $"{Label}: <final>{Final}</final> [<base>{Base}</base> + <bonus>{Bonus}</bonus>]";

            // 只有Debuff
            if (Bonus == 0 && Debuff != 0)
                return $"{Label}: <final>{Final}</final> [<base>{Base}</base> - <debuff>{Debuff}</debuff>]";

            // 两者都有
            return $"{Label}: <final>{Final}</final> [<base>{Base}</base> + <bonus>{Bonus}</bonus> - <debuff>{Debuff}</debuff>]";
        }
    }
}