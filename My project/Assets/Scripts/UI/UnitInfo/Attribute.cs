using UnityEngine;

namespace UI.UnitInfo
{
    public class Attribute : MonoBehaviour
    {
        [SerializeField] protected TMPro.TextMeshProUGUI attributeNameText;
        [SerializeField] protected TMPro.TextMeshProUGUI attributeValueText;

        protected Units.Attribute attribute;
        protected string finalColor = "#FFD700"; // 金色
        protected string baseColor = "#FFFFFF";  // 白色
        protected string flatColor = "#00FF00";  // 绿色
        protected string percentColor = "#FFA500"; // 橙色

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

            string valueText = $"<color={finalColor}>{attribute.finalValue:F0}</color> [<color={baseColor}>{baseValue:F0}</color>";
            if (flatSum != 0f)
                valueText += $" +<color={flatColor}>{flatSum:F0}</color>";
            if (percentSum != 0f)
                valueText += $" +<color={percentColor}>{percentSum:F0}%</color>";
            valueText += "]";

            attributeValueText.text = valueText;
        }
    }
}