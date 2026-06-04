using System.Globalization;
using System.Text;
using UnityEngine;

namespace UI.UnitInfo
{
    public static class RichTextUtils
    {
        private static string FormatValue(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public static string BuildValueText(
            Units.Attribute attr,
            Color baseColor,
            Color positiveColor,
            Color negativeColor,
            float? currentValue = null)
        {
            float baseValue = attr.BaseValue;

            float flatSum = 0f;
            foreach (var v in attr.FlatModifiers)
                flatSum += v;

            float percentSum = 0f;
            foreach (var v in attr.PercentageModifiers)
                percentSum += v;

            float percentValue = (baseValue + flatSum) * (percentSum / 100f);

            Color finalColor =
                attr.finalValue > baseValue ? positiveColor :
                attr.finalValue < baseValue ? negativeColor :
                baseColor;

            string hexBase = ColorUtility.ToHtmlStringRGB(baseColor);
            string hexPos = ColorUtility.ToHtmlStringRGB(positiveColor);
            string hexNeg = ColorUtility.ToHtmlStringRGB(negativeColor);
            string hexFinal = ColorUtility.ToHtmlStringRGB(finalColor);

            StringBuilder sb = new StringBuilder();

            if (currentValue.HasValue)
            {
                // Health 用： current / final
                sb.Append($"<color=#{hexBase}>{FormatValue(currentValue.Value)}</color>/<color=#{hexFinal}>{FormatValue(attr.finalValue)}</color> ");
            }
            else
            {
                sb.Append($"<color=#{hexFinal}>{FormatValue(attr.finalValue)}</color> ");
            }

            sb.Append($"[<color=#{hexBase}>{FormatValue(baseValue)}</color>");

            if (flatSum != 0f)
            {
                bool positive = flatSum > 0;
                string hex = positive ? hexPos : hexNeg;
                sb.Append($" <color=#{hex}>{(positive ? "+" : "")}{FormatValue(flatSum)}</color>");
            }

            if (percentValue != 0f)
            {
                bool positive = percentValue > 0;
                string hex = positive ? hexPos : hexNeg;
                sb.Append($" <color=#{hex}>{(positive ? "+" : "")}{FormatValue(percentValue)}({FormatValue(percentSum)}%)</color>");
            }

            sb.Append("]");

            return sb.ToString();
        }
    }
}