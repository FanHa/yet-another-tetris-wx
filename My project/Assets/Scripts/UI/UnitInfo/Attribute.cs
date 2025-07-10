using UnityEngine;

namespace UI.UnitInfo
{
    public class Attribute : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI attributeNameText;
        [SerializeField] private TMPro.TextMeshProUGUI attributeValueText;

        public void SetAttribute(string name, string value)
        {
            attributeNameText.text = name;
            attributeValueText.text = value;
        }
    }
}