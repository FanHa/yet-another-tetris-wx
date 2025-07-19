using System;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Skill : MonoBehaviour
    {
        private Image skillIcon;
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;
        private Units.Skills.Skill skill;
        public event Action<Units.Skills.Skill> OnClicked;

        private void Awake()
        {
            skillIcon = GetComponent<Image>();
            Button button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
        }
        public void SetSkill(Units.Skills.Skill skill)
        {
            this.skill = skill;
            skillIcon.sprite = cellTypeResourceMapping.GetSprite(skill.CellTypeId);
        }
        private void HandleClick()
        {
            OnClicked?.Invoke(this.skill);
        }
    }
}