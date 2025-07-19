using System;
using System.Collections.Generic;
using Model.Tetri;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class UnitInfo : MonoBehaviour
    {
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;

        [Header("属性")]
        [SerializeField] private Transform attributeRoot;
        [SerializeField] private UI.UnitInfo.Attribute unitAttributePrefab;

        [Header("技能")]
        [SerializeField] private UI.UnitInfo.Skill unitSkillPrefab;
        [SerializeField] private Transform skillRoot;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;

        [Header("Buff")]
        [SerializeField] private UI.UnitInfo.Buff buffInfoPrefab;
        [SerializeField] private Transform buffRoot;
        [SerializeField] private TMPro.TextMeshProUGUI buffDescriptionText;

        [Header("运行时设置")]
        [SerializeField] private Utils.CameraFollower unitInfoCamera;

        private Units.Unit currentUnit;
        private const float refreshInterval = 0.2f;
        private float refreshTimer = 0f;

        public void Awake()
        {
            closeButton.onClick.AddListener(HideUnitInfo);
        }

        public void Start()
        {
            panel.SetActive(false);
        }

        void Update()
        {
            if (panel.activeSelf && currentUnit != null)
            {
                refreshTimer += Time.deltaTime;
                if (refreshTimer >= refreshInterval)
                {
                    RefreshInfo(currentUnit);
                    refreshTimer = 0f;
                }
            }
        }

        public void HideUnitInfo()
        {
            currentUnit = null;
            panel.SetActive(false);
        }

        public void ShowUnitInfo(Units.Unit unit)
        {
            currentUnit = unit;
            panel.SetActive(true);
            RefreshInfo(unit);
            refreshTimer = 0f;
            
        }

        public void RefreshInfo(Units.Unit unit)
        {
            foreach (Transform child in attributeRoot)
                Destroy(child.gameObject);
            foreach (Transform child in skillRoot)
                Destroy(child.gameObject);
            foreach (Transform child in buffRoot)
                Destroy(child.gameObject);

            unitInfoCamera.SetTarget(unit.transform);
            
            Units.Attributes attributes = unit.Attributes;
             // 1. 先显示 MaxHealth（CurrentHealth / MaxHealth）
            var maxHealthAttr = attributes.MaxHealth;
            var instantiatedMaxHealth = Instantiate(unitAttributePrefab, attributeRoot);

            float healthBaseValue = maxHealthAttr.BaseValue;
            float healthFlatSum = 0f;
            foreach (var v in maxHealthAttr.FlatModifiers)
                healthFlatSum += v;
            float healthPercentSum = 0f;
            foreach (var v in maxHealthAttr.PercentageModifiers)
                healthPercentSum += v;

            string finalColor   = "#FFD700"; // 金色
            string baseColor    = "#FFFFFF"; // 白色
            string flatColor    = "#00FF00"; // 绿色
            string percentColor = "#FFA500"; // 橙色

            string healthValueText = $"<color={finalColor}>{unit.Attributes.CurrentHealth:F0}</color>/<color={finalColor}>{maxHealthAttr.finalValue:F0}</color> [<color={baseColor}>{healthBaseValue:F0}</color>";
            if (healthFlatSum != 0f)
                healthValueText += $" +<color={flatColor}>{healthFlatSum:F0}</color>";
            if (healthPercentSum != 0f)
                healthValueText += $" +<color={percentColor}>{healthPercentSum:F0}%</color>";
            healthValueText += "]";

            instantiatedMaxHealth.SetAttribute("生命值", healthValueText);

            var AttributeToShow = new List<Units.Attribute>(
                new[]
                {
                    attributes.MoveSpeed,
                    attributes.AttackPower,
                    attributes.AttacksPerTenSeconds,
                    attributes.EnergyPerTick
                }
            );
            foreach (Units.Attribute attribute in AttributeToShow)
            {
                var instantiatedAttribute = Instantiate(unitAttributePrefab, attributeRoot);
                float baseValue = attribute.BaseValue; // 你需要加这个getter
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

                instantiatedAttribute.SetAttribute(attribute.Name, valueText);


            }

            IReadOnlyList<Units.Skills.Skill> skills = unit.GetSkills();
            foreach (Units.Skills.Skill skill in skills)
            {
                UI.UnitInfo.Skill skillInfo = Instantiate(unitSkillPrefab, skillRoot);
                skillInfo.SetSkill(skill);
                skillInfo.OnClicked += ShowSkillDescription;
            }

            List<Units.Buffs.Buff> buffs = unit.GetActiveBuffs();
            if (buffs.Count > 0)
            {
                foreach (Units.Buffs.Buff buff in buffs)
                {
                    UI.UnitInfo.Buff buffInfo = Instantiate(buffInfoPrefab, buffRoot);
                    buffInfo.SetBuff(buff);
                    buffInfo.OnBuffClicked += ShowBuffDescription;
                }
            }
        }

        public void ShowBuffDescription(Units.Buffs.Buff buff)
        {
            buffDescriptionText.text = "<b>" + buff.Name() + "</b>: " + buff.Description();
        }
        public void ShowSkillDescription(Units.Skills.Skill skill)
        {
            skillDescriptionText.text = "<b>" + skill.Name() + "</b>: " + skill.Description();
        }
    }
}