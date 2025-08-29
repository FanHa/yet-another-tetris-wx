using UnityEngine;

namespace UI.UnitInfo
{
    public class Attribute : MonoBehaviour
    {
        [SerializeField] protected TMPro.TextMeshProUGUI attributeNameText;
        [SerializeField] protected TMPro.TextMeshProUGUI attributeValueText;

        protected Units.Attribute attribute;

        [Header("颜色配置")]
        [SerializeField] private Color baseColor;
        [SerializeField] private Color positiveColor; // 用于正加成
        [SerializeField] private Color negativeColor; // 用于负加成


        void Update()
        {
            if (attribute != null)
                UpdateValueText();
        }

        public void SetAttribute(Units.Attribute attribute)
        {
            this.attribute = attribute;
            attributeNameText.text = attribute.Name;
            UpdateValueText();
        }

        private void UpdateValueText()
        {
            float baseValue = attribute.BaseValue;
            float flatSum = 0f;
            foreach (var v in attribute.FlatModifiers)
                flatSum += v;
            float percentSum = 0f;
            foreach (var v in attribute.PercentageModifiers)
                percentSum += v;
            
            float percentValue = (baseValue + flatSum) * (percentSum / 100f);

            Color finalColor;
            if (attribute.finalValue > baseValue)
                finalColor = positiveColor;
            else if (attribute.finalValue < baseValue)
                finalColor = negativeColor;
            else
                finalColor = baseColor;

            string valueText = $"<color=#{ColorUtility.ToHtmlStringRGB(finalColor)}>{attribute.finalValue:F0}</color> [<color=#{ColorUtility.ToHtmlStringRGB(baseColor)}>{baseValue:F0}</color>";

            if (flatSum != 0f)
            {
                var color = flatSum > 0 ? positiveColor : negativeColor;
                valueText += $" <color=#{ColorUtility.ToHtmlStringRGB(color)}>{(flatSum > 0 ? "+" : "")}{flatSum:F0}</color>";
            }
            if (percentValue != 0f)
            {
                var color = percentValue > 0 ? positiveColor : negativeColor;
                valueText += $" <color=#{ColorUtility.ToHtmlStringRGB(color)}>{(percentValue > 0 ? "+" : "")}{percentValue:F0}({percentSum:F0}%)</color>";
            }
            valueText += "]";

            attributeValueText.text = valueText;
        }
    }
}