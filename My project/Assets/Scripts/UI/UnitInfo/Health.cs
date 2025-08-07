using UnityEngine;

namespace UI.UnitInfo
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI attributeNameText;
        [SerializeField] private TMPro.TextMeshProUGUI attributeValueText;
        private Units.Attributes attributes;
        private Units.Attribute maxHealthAttr;

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

            string finalColor = "#FFD700"; // 金色
            string baseColor = "#FFFFFF";  // 白色
            string flatColor = "#00FF00";  // 绿色
            string percentColor = "#FFA500"; // 橙色

            string valueText = $"<color={finalColor}>{current:F0}</color>/<color={finalColor}>{max:F0}</color> [<color={baseColor}>{baseValue:F0}</color>";
            if (flatSum != 0f)
                valueText += $" +<color={flatColor}>{flatSum:F0}</color>";
            if (percentSum != 0f)
                valueText += $" +<color={percentColor}>{percentSum:F0}%</color>";
            valueText += "]";

            attributeValueText.text = valueText;
        }
    }
}