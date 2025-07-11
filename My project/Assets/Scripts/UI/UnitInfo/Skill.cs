using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Skill : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMPro.TextMeshProUGUI descriptionText;

        public void SetSkill(string name, string description, Sprite icon)
        {
            descriptionText.text = "<b>" + name + "</b>: " + description;
            skillIcon.sprite = icon;
        }
    }
}