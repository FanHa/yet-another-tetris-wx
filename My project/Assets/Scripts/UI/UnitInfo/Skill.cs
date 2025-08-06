using System;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Skill : MonoBehaviour
    {
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Slider energySlider;
        private Units.Skills.Skill skill;
        public event Action<Units.Skills.Skill> OnClicked;

        private void Awake()
        {
            button.onClick.AddListener(HandleClick);
        }

        void Update()
        {
            if (skill is Units.Skills.ActiveSkill activeSkill)
            {
                energySlider.value = activeSkill.CurrentEnergy;
            }
        }

        public void SetSkill(Units.Skills.Skill skill)
        {
            this.skill = skill;
            icon.sprite = cellTypeResourceMapping.GetSprite(skill.CellTypeId);
            if (skill is Units.Skills.ActiveSkill activeSkill)
            {
                energySlider.gameObject.SetActive(true);
                energySlider.maxValue = activeSkill.RequiredEnergy;
                energySlider.value = 0f; // 初始值为0
            }
            else
            {
                energySlider.gameObject.SetActive(false);
            }
        }
        private void HandleClick()
        {
            OnClicked?.Invoke(this.skill);
        }
    }
}