using System;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Skill : MonoBehaviour
    {
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
                energySlider.value = activeSkill.EnergyProgress;
            }
        }

        public void SetSkill(Units.Skills.Skill skill)
        {
            this.skill = skill;
            icon.sprite = skill.Definition.Icon;
            if (skill is Units.Skills.ActiveSkill activeSkill)
            {
                energySlider.gameObject.SetActive(true);
                energySlider.minValue = 0f;
                energySlider.maxValue = 1f;
                energySlider.value = activeSkill.EnergyProgress;
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