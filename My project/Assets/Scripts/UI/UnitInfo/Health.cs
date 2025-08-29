using UnityEngine;

namespace UI.UnitInfo
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI attributeNameText;
        [SerializeField] private TMPro.TextMeshProUGUI attributeValueText;
        private Units.Attributes attributes;
        private Units.Attribute maxHealthAttr;

        [Header("颜色配置")]
        [SerializeField] private Color baseColor;
        [SerializeField] private Color positiveColor;
        [SerializeField] private Color negativeColor;

        public void BindAttributes(Units.Attributes attributes)
        {
            this.attributes = attributes;
            this.maxHealthAttr = attributes.MaxHealth;
            attributeNameText.text = "生命值";
        }

        private void Update()
        {
            if (attributes == null || maxHealthAttr == null) return;

            float current = attributes.CurrentHealth;
            float max = maxHealthAttr.finalValue;
            float baseValue = maxHealthAttr.BaseValue;
            float flatSum = 0f;
            foreach (var v in maxHealthAttr.FlatModifiers)
                flatSum += v;
            float percentSum = 0f;
            foreach (var v in maxHealthAttr.PercentageModifiers)
                percentSum += v;

            float percentValue = (baseValue + flatSum) * (percentSum / 100f);

            Color finalColor;
            if (max > baseValue)
                finalColor = positiveColor;
            else if (max < baseValue)
                finalColor = negativeColor;
            else
                finalColor = baseColor;

            string valueText = $"<color=#{ColorUtility.ToHtmlStringRGB(baseColor)}>{current:F0}</color>/<color=#{ColorUtility.ToHtmlStringRGB(finalColor)}>{max:F0}</color> [<color=#{ColorUtility.ToHtmlStringRGB(baseColor)}>{baseValue:F0}</color>";
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