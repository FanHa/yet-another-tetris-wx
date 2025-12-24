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
        public Action OnClosed;
        [SerializeField] private Button closeButton;

        [Header("属性")]
        [SerializeField] private Transform attributeRoot;
        [SerializeField] private UI.UnitInfo.Attribute unitAttributePrefab;
        [SerializeField] private UI.UnitInfo.Health healthPrefab;

        [Header("Affinity")]
        [SerializeField] private UI.UnitInfo.Affinity affinityInfoPrefab;
        [SerializeField] private Transform affinityRoot;

        [Header("技能")]
        [SerializeField] private UI.UnitInfo.Skill unitSkillPrefab;
        [SerializeField] private Transform skillRoot;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;

        [Header("Buff")]
        [SerializeField] private UI.UnitInfo.Buff buffInfoPrefab;
        [SerializeField] private Transform buffRoot;
        [SerializeField] private TMPro.TextMeshProUGUI buffDescriptionText;

        [Header("实例设置")]
        [SerializeField] private Utils.CameraFollower buffSourceUnitCamera;
        [SerializeField] private Utils.CameraFollower currentUnitCamera;

        private Units.Unit currentUnit;

        public void Awake()
        {
            closeButton.onClick.AddListener(HideUnitInfo);
        }

        public void HideUnitInfo()
        {
            currentUnit = null;
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }

        public void ShowUnitInfo(Units.Unit unit)
        {
            currentUnit = unit;
            gameObject.SetActive(true);
            RefreshInfo();
            unit.BuffAdded += HandleUnitBuffChanged;
            unit.BuffRemoved += HandleUnitBuffChanged;
            
        }
        
        private void HandleUnitBuffChanged(Units.Buffs.Buff buff)
        {
            RefreshBuffInfo();
        }

        private void RefreshBuffInfo()
        {
            buffDescriptionText.text = ""; // 清空原有内容
            if (currentUnit == null) return;

            foreach (Transform child in buffRoot)
                Destroy(child.gameObject);

            List<Units.Buffs.Buff> buffs = currentUnit.GetActiveBuffs();

            foreach (Units.Buffs.Buff buff in buffs)
            {
                UI.UnitInfo.Buff buffInfo = Instantiate(buffInfoPrefab, buffRoot);
                buffInfo.SetBuff(buff);
                buffInfo.OnBuffClicked += ShowBuffDescription;
            }
            
        }

        private void RefreshSkillInfo()
        {
            skillDescriptionText.text = ""; // 清空原有内容
            foreach (Transform child in skillRoot)
                Destroy(child.gameObject);
            IReadOnlyList<Units.Skills.Skill> skills = currentUnit.GetSkills();
            foreach (Units.Skills.Skill skill in skills)
            {
                UI.UnitInfo.Skill skillInfo = Instantiate(unitSkillPrefab, skillRoot);
                skillInfo.SetSkill(skill);
                skillInfo.OnClicked += ShowSkillDescription;
            }
        }

        private void RefreshAttributes()
        {
            if (currentUnit == null) return;

            foreach (Transform child in attributeRoot)
                Destroy(child.gameObject);

            Units.Attributes attributes = currentUnit.Attributes;

            var instantiatedHealth = Instantiate(healthPrefab, attributeRoot);
            instantiatedHealth.BindAttributes(currentUnit.Attributes);

            var AttributeToShow = new List<Units.Attribute>(
                new[]
                {
                    attributes.MoveSpeed,
                    attributes.AttackPower,
                    attributes.AttacksPerTenSeconds,
                    attributes.EnergyPerSecond
                }
            );
            foreach (Units.Attribute attribute in AttributeToShow)
            {
                var instantiatedAttribute = Instantiate(unitAttributePrefab, attributeRoot);
                instantiatedAttribute.SetAttribute(attribute);
            }
        }
        private void RefreshAffinity()
        {
            if (currentUnit == null) return;

            foreach (Transform child in affinityRoot)
                Destroy(child.gameObject);

            Dictionary<Model.Tetri.AffinityType, int> affinities = currentUnit.CellCounts;
            foreach (var affinity in affinities)
            {
                if (affinity.Key == Model.Tetri.AffinityType.None)
                    continue;
                UI.UnitInfo.Affinity affinityInfo = Instantiate(affinityInfoPrefab, affinityRoot);
                affinityInfo.SetAffinity(affinity.Key, affinity.Value);
            }
        }

        private void RefreshInfo()
        {
            currentUnitCamera.SetTarget(currentUnit.transform);

            RefreshAttributes();
            RefreshBuffInfo();
            RefreshSkillInfo();
            RefreshAffinity();
        }

        public void ShowBuffDescription(Units.Buffs.Buff buff)
        {
            buffDescriptionText.text = "<b>" + buff.Name() + "</b>: " + buff.Description();
            buffSourceUnitCamera.SetTarget(buff.SourceUnit.transform);
            
        }
        public void ShowSkillDescription(Units.Skills.Skill skill)
        {
            string desc = skill.Description();

            // 替换自定义标签为富文本颜色
            desc = desc.Replace("<base>", "<color=#FFFFFF>")
                    .Replace("</base>", "</color>")
                    .Replace("<bonus>", "<color=#00FF66>")
                    .Replace("</bonus>", "</color>")
                    .Replace("<debuff>", "<color=#FF5555>")
                    .Replace("</debuff>", "</color>")
                    .Replace("<final>", "<color=#FFD700>")
                    .Replace("</final>", "</color>");

            skillDescriptionText.text = "<b>" + skill.Name() + "</b>: " + desc;
        }
    }
}