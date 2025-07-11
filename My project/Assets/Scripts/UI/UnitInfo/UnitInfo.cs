using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class UnitInfo : MonoBehaviour
    {
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;

        [SerializeField] private Transform attributeRoot;
        [SerializeField] private UI.UnitInfo.Attribute unitAttributePrefab;
        [SerializeField] private UI.UnitInfo.Skill unitSkillPrefab;
        [SerializeField] private Transform skillRoot;

        [Header("运行时设置")]
        [SerializeField] private Utils.CameraFollower unitInfoCamera;

        public void Awake()
        {
            closeButton.onClick.AddListener(HideUnitInfo);
        }

        private void HideUnitInfo()
        {
            panel.SetActive(false);
        }

        public void ShowUnitInfo(Units.Unit unit)
        {
            foreach (Transform child in attributeRoot)
                Destroy(child.gameObject);
            foreach (Transform child in skillRoot)
                Destroy(child.gameObject);
                
            unitInfoCamera.SetTarget(unit.transform);
            panel.SetActive(true);
            Units.Attributes attributes = unit.Attributes;
            var AttributeToShow = new List<Units.Attribute>(
                new[]
                {
                    attributes.MoveSpeed,
                    attributes.AttackPower,
                    attributes.MaxHealth,
                    attributes.AttacksPerTenSeconds,
                    attributes.EnergyPerTick
                }
            );
            foreach (var attribute in AttributeToShow)
            {
                var instantiatedAttribute = Instantiate(unitAttributePrefab, attributeRoot);
                instantiatedAttribute.SetAttribute(attribute.Name, attribute.finalValue.ToString());
            }

            IReadOnlyList<Units.Skills.Skill> skills = unit.GetSkills();
            foreach (Units.Skills.Skill skill in skills)
            {
                UI.UnitInfo.Skill instantiatedSkill = Instantiate(unitSkillPrefab, skillRoot);
                instantiatedSkill.SetSkill(skill.Name(), skill.Description(), cellTypeResourceMapping.GetSprite(skill.CellTypeId));
            }
        }
    }
}