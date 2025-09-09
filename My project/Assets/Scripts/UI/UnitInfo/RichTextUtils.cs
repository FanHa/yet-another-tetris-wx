using UnityEngine;
using System.Collections.Generic;

namespace UI.UnitInfo
{
    public static class RichTextUtils
    {
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

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (currentValue.HasValue)
            {
                // Health 用： current / final
                sb.Append($"<color=#{hexBase}>{currentValue.Value:F0}</color>/<color=#{hexFinal}>{attr.finalValue:F0}</color> ");
            }
            else
            {
                sb.Append($"<color=#{hexFinal}>{attr.finalValue:F0}</color> ");
            }

            sb.Append($"[<color=#{hexBase}>{baseValue:F0}</color>");

            if (flatSum != 0f)
            {
                bool positive = flatSum > 0;
                string hex = positive ? hexPos : hexNeg;
                sb.Append($" <color=#{hex}>{(positive ? "+" : "")}{flatSum:F0}</color>");
            }

            if (percentValue != 0f)
            {
                bool positive = percentValue > 0;
                string hex = positive ? hexPos : hexNeg;
                sb.Append($" <color=#{hex}>{(positive ? "+" : "")}{percentValue:F0}({percentSum:F0}%)</color>");
            }

            sb.Append("]");

            return sb.ToString();
        }
    }
}